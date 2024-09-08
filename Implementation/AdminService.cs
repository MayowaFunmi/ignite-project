using System.Net;
using ignite_project.Constants;
using ignite_project.Data;
using ignite_project.DTOs;
using ignite_project.Interface;
using ignite_project.Models;
using ignite_project.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WatchDog;

namespace ignite_project.Implementation
{
  public class AdminService(UserManager<ApplicationUser> userManager, ILoggerManager logger, IConfiguration configuration, ApplicationDbContext context) : IAdminService
  {
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILoggerManager _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly ApplicationDbContext _context = context;

    public async Task<GenericResponse> InviteCode()
    {
      try
      {
        _logger.LogInfo("Attempting to generate invitation code");
        var code = GenerateCode.InvitationCode();
        var codeExists = await _context.SignUpCodes.AnyAsync(c => c.Code == code);
        if (codeExists)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = $"Code {code} already exists",
          };
        }
        var saveCode = new SignUpCode
        {
          Code = code,
        };
        await _context.SignUpCodes.AddAsync(saveCode);
        var res = await _context.SaveChangesAsync();
        if (res > 0) 
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "Invitation code generated successfully",
            Data = code
          };
        } else 
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Failed to save code",
          };
        }
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

    public async Task<GenericResponse> GetUsersByRole(string roleName)
    {
      if (string.IsNullOrEmpty(roleName))
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "roleName cannot be empty"
        };
      }
      try
      {
        _logger.LogError($"Attempting to get users having role : {roleName}");
        
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "Role does not exist"
          };
        }

        var users = await _context.Users
                  .AsNoTracking()
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
                      ProfilePicture = user.ProfilePicture ?? string.Empty,
                      ActiveSubscription = user.ActiveSubscription,
                      IsActive = user.IsActive,
                      Ratings = user.Ratings,
                      WithdrawalRequests = user.WithdrawalRequests,
                      TotalWithdrawals = user.TotalWithdrawals,
                      SignupCode = user.SignupCode ?? string.Empty,
                      CreatedAt = user.CreatedAt,
                      UpdatedAt = user.UpdatedAt,
                    },
                    Roles = _context.UserRoles
                      .Where(ur => ur.UserId == user.Id && ur.RoleId == role.Id)
                      .Join(_context.Roles,
                          ur => ur.RoleId,
                          r => r.Id,
                          (ur, r) => r.Name)
                      .ToList()
                  })
                  .ToListAsync();

        if (users.Count == 0)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = $"users with role {roleName} not found"
          };
        }
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = $"users with role {roleName} retrieved successfully",
          Data = users
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

    public async Task<GenericResponse> GetUserByCode(string code)
    {
      if (string.IsNullOrEmpty(code))
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "userId cannot be empty"
        };
      }

      try
      {
        _logger.LogError($"Attempting to get user with id {code}");
        var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == code)
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
                    ProfilePicture = user.ProfilePicture ?? string.Empty,
                    ActiveSubscription = user.ActiveSubscription,
                    IsActive = user.IsActive,
                    Ratings = user.Ratings,
                    WithdrawalRequests = user.WithdrawalRequests,
                    TotalWithdrawals = user.TotalWithdrawals,
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
      catch (System.Exception)
      {
        
        throw;
      }
    }

    public async Task<PaginationResponse> GetAllUsers(int page, int pageSize)
    {
      try
      {
        _logger.LogInfo("Attempting to get all users");
        var usersWithRoles = await _context.Users
                .Select(user => new
                {
                  User = new UserDetails
                  {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    InvitationCode = user.InvitationCode ?? string.Empty,
                    WalletAddress = user.WalletAddress ?? string.Empty,
                    CryptoCoin = user.CryptoCoin ?? string.Empty,
                    WalletBallance = user.WalletBallance,
                    Location = user.Location ?? string.Empty,
                    ProfilePicture = user.ProfilePicture ?? string.Empty,
                    ActiveSubscription = user.ActiveSubscription,
                    IsActive = user.IsActive,
                    Ratings = user.Ratings,
                    WithdrawalRequests = user.WithdrawalRequests,
                    TotalWithdrawals = user.TotalWithdrawals,
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
                .OrderBy(u => u.User.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
          var userCount = await _context.Users.AsNoTracking().CountAsync();
          var paginationMetaData = new PaginationMetaData(page, pageSize, userCount);

          return new PaginationResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "All Users retrieved successfully",
            Data = usersWithRoles,
            Pagination = paginationMetaData
          };
      }
      catch (Exception ex)
      {
        _logger.LogError("Something went wrong while generating invitation code");
        WatchLogger.LogError("Something went wrong while generating invitation code");
        return new PaginationResponse
        {
          Status = HttpStatusCode.InternalServerError.ToString(),
          Message = $"Something went wrong while generating invitation code - {ex.Message}",
        };
      }
    }

    public async Task<GenericResponse> ApproveWithdrawalRequest(ApproveRequestDto requestDto)
    {
      if (!string.IsNullOrEmpty(requestDto.UserId) || 
          requestDto.Amount == 0 ||
          !string.IsNullOrEmpty(requestDto.RequestId)
        )
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "All fields cannot be empty"
        };
      }
      try
      {
        _logger.LogInfo("Attempting to approve withdrawal request");
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == requestDto.UserId);
        if (user == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "User not found"
          };
        }

        var request = await _context.WithdrawalRequests.FirstOrDefaultAsync(w => w.Id.ToString() == requestDto.RequestId && w.UserId == user.Id);
        if (request == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "withdrawal request for user does not exist"
          };
        }

        if (requestDto.Status)
        {
          user.WalletBallance -= requestDto.Amount;
          user.ApprovedRequests ++;
          user.TotalWithdrawals += requestDto.Amount;
          request.Status = WithdrawalStatus.APPROVED;
          request.ApprovedAt = DateTime.UtcNow;
          _context.Users.Update(user);
          _context.WithdrawalRequests.Update(request);
          var res = await _context.SaveChangesAsync();
          if (res > 0)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "withdrawal request approved successfully"
            };
          }
          else
          {
            request.Status = WithdrawalStatus.INCOMPLETE;
            _context.WithdrawalRequests.Update(request);
            await _context.SaveChangesAsync();
            return new GenericResponse
            {
              Status = HttpStatusCode.BadRequest.ToString(),
              Message = "failed to approve withdrawal request"
            };
          }
        }
        else
        {
          user.RejectedRequests ++;
          request.Status = WithdrawalStatus.REJECTED;
          _context.WithdrawalRequests.Update(request);
          _context.Users.Update(user);
          await _context.SaveChangesAsync();
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "withdrawal request rejected"
          };
        }
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
  }
}