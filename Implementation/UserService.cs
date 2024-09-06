using System.Net;
using System.Text.RegularExpressions;
using ignite_project.Data;
using ignite_project.DTOs;
using ignite_project.Interface;
using ignite_project.Models;
using ignite_project.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using WatchDog;

namespace ignite_project.Implementation
{
  public class UserService(ILoggerManager logger, IConfiguration configuration, ApplicationDbContext context) : IUserService
  {
    private readonly ILoggerManager _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly ApplicationDbContext _context = context;

    public async Task<GenericResponse> AddUserWalletAddress(AddWalletDto addWalletDto)
    {
      if (string.IsNullOrEmpty(addWalletDto.UserId) || string.IsNullOrEmpty(addWalletDto.WalletAddress))
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "UserId or Wallet address cannot be empty"
        };
      }
      try
      {
        _logger.LogInfo("Attempting to add user wallet address");
        var verifyAddress = VerifyWallet.ValidateWalletAddress(addWalletDto.WalletAddress);
        if (verifyAddress == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Wallet address cannot be verified. Contact Admin"
          };
        }
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == addWalletDto.UserId);
        if (user == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "user not found"
          };
        }

        user.WalletAddress = addWalletDto.WalletAddress;
        user.CryptoCoin = verifyAddress.CoinType;
        _context.Users.Update(user);
        var res = await _context.SaveChangesAsync();
        if (res > 0)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "wallet address added to user"
          };
        }
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "failed to add wallet address to user"
        };
      }
      catch (Exception ex)
      {
        _logger.LogError("Something went wrong while generating invitation code");
        WatchLogger.LogError("Something went wrong while generating invitation code");
        return new GenericResponse
        {
          Status = HttpStatusCode.InternalServerError.ToString(),
          Message = $"Something went wrong while generating invitation code - {ex.Message}",
        };
      }
    }

    public async Task<GenericResponse> AddUserProfilePicture(AddProfilePictureDto pictureDto)
    {
      try
      {
        _logger.LogInfo("Attempting to add user profile picture");

        if (string.IsNullOrEmpty(pictureDto.UserId))
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "UserId cannot be empty"
          };
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == pictureDto.UserId);
        if (user == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "user not found"
          };
        }

        return await AddProfilePicture(pictureDto.File, user.UserName!);
      }
      catch (Exception ex)
      {
        _logger.LogError("Something went wrong while generating invitation code");
        WatchLogger.LogError("Something went wrong while generating invitation code");
        return new GenericResponse
        {
          Status = HttpStatusCode.InternalServerError.ToString(),
          Message = $"Something went wrong while generating invitation code - {ex.Message}",
        };
      }
    }

    public async Task<GenericResponse> GetUserById(string userId)
    {
      if (string.IsNullOrEmpty(userId))
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "userId cannot be empty"
        };
      }
      try
      {
        _logger.LogError($"Attempting to get user with id {userId}");
        //var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(user => new
                {
                  User = new UserDetails
                  {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    InvitationCode = user.InvitationCode ?? string.Empty,
                    WalletAddress = user.WalletAddress ?? string.Empty,
                    WalletBallance = user.WalletBallance,
                    Location = user.Location ?? string.Empty,
                    ActiveSubscription = user.ActiveSubscription,
                    IsActive = user.IsActive,
                    Ratings = user.Ratings,
                    SignupCode = user.SignupCode ?? string.Empty,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                  },
                  Roles = _context.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Join(_context.Roles,
                        ur => ur.RoleId,
                        r => r.Id,
                        (ur, r) => r.Name)
                    .ToList()
                })
                .FirstOrDefaultAsync();
        if (user == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "user not found"
          };
        }
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "User retrieved successfully",
          Data = user
        };
      }
      catch (Exception ex)
      {
        _logger.LogError("Something went wrong while generating invitation code");
        WatchLogger.LogError("Something went wrong while generating invitation code");
        return new GenericResponse
        {
          Status = HttpStatusCode.InternalServerError.ToString(),
          Message = $"Something went wrong while generating invitation code - {ex.Message}",
        };
      }
    }

    private static async Task<GenericResponse> AddProfilePicture(IFormFile file, string username)
    {
      try
      {
        if (file == null || file.Length == 0)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "You cannot upload an empty file",
          };
        }

        if (string.IsNullOrEmpty(username))
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "username cannot be empty",
          };
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".jpg" && extension != ".jpeg")
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Only JPG or JEPG photos are allowed",
          };
        }
        var baseDirectory = Directory.GetCurrentDirectory();
        var outputPath = Path.Combine(baseDirectory, $"FileBucket/{username}.webp");
        
        await ConvertJpegToWebpAsync(file, outputPath);
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "Profile picture uploaded successfully",
          Data = outputPath
        };
      }
      catch (InvalidOperationException ex)
      {
        WatchLogger.LogError("Something went wrong while generating invitation code");
        return new GenericResponse
        {
          Status = HttpStatusCode.InternalServerError.ToString(),
          Message = $"Something went wrong while generating invitation code - {ex.Message}",
        };
      }
      
    }

    private static async Task ConvertJpegToWebpAsync(IFormFile inputFile, string outputPath)
    {
      try
      {
        if (inputFile == null || inputFile.Length == 0 || string.IsNullOrEmpty(outputPath))
          throw new ArgumentException("Invalid image file");

        using var stream = inputFile.OpenReadStream();
        using Image image = Image.Load<Rgba32>(stream);
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
          Directory.CreateDirectory(directory);
        }
        var webpEncoder = new WebpEncoder()
        {
          Quality = 50
        };
        await image.SaveAsync(outputPath, webpEncoder);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("An unexpected error occurred while converting the image.", ex);
      }
    }

    private static bool IsValidBitcoinAddress(string walletAddress)
    {
      var regex = new Regex(@"^[13][a-km-zA-HJ-NP-Z1-9]{25,34}$");
      return regex.IsMatch(walletAddress);
    }

    private static bool IsValidEthereumAddress(string walletAddress)
    {
      return walletAddress.StartsWith("0x") && walletAddress.Length == 42;
    }
  }
}