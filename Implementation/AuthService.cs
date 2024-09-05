using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using ignite_project.Configurations;
using ignite_project.Constants;
using ignite_project.Data;
using ignite_project.DTOs;
using ignite_project.Interface;
using ignite_project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WatchDog;

namespace ignite_project.Implementation
{
  public class AuthService(UserManager<ApplicationUser> userManager, ILoggerManager logger, IConfiguration configuration, ApplicationDbContext context) : IAuthService
  {
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILoggerManager _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly ApplicationDbContext _context = context;

    public class LoginResponse
    {
      public string Token { get; set; } = string.Empty;
      public string Username { get; set; } = string.Empty;
      public IList<string> Roles { get; set; } = [];
    }

    public async Task<GenericResponse> CreateUser(RegisterDto register)
    {
      try
      {
        _logger.LogError("Attempting to sign up user");
        if (
          string.IsNullOrEmpty(register.Username) || 
          string.IsNullOrEmpty(register.InvitationCode) ||
          string.IsNullOrEmpty(register.PhoneNumber) ||
          string.IsNullOrEmpty(register.Location) ||
          string.IsNullOrEmpty(register.Password)
          )
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "All fields are required"
          };
        }
        var userExist = await _userManager.Users.AnyAsync(u => u.UserName == register.Username);
        if (userExist)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Username already exist"
          };
        }
        var storedCode = await _context.SignUpCodes.FirstOrDefaultAsync(c => c.Code == register.InvitationCode);
        if (storedCode == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = $"This code {register.InvitationCode} is invalid"
          };
        }
        if (storedCode.IsExpired)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = $"This code {register.InvitationCode} is has expired"
          };
        }

        var user = new ApplicationUser
        {
          UserName = register.Username,
          InvitationCode = register.InvitationCode,
          PhoneNumber = register.PhoneNumber,
          Location = register.Location,
          WalletBallance = 10,
          SignupCode = storedCode.Code,
        };

        var result = await _userManager.CreateAsync(user, register.Password);
        if (result.Succeeded)
        {
          await _userManager.AddToRolesAsync(user, [UserRoles.User]);
          _context.SignUpCodes.Remove(storedCode);
          await _context.SaveChangesAsync();
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "User created successfully"
          };
        }
        else
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Failed to create user"
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

    public async Task<GenericResponse> AuthenticateUser(LoginDto loginDto)
    {
      try
      {
        _logger.LogInfo("Attempting to log in user");
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username);
        if (user == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "User not found"
          };
        }

        if (!user.IsActive)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "User is inactive, cannot login"
          };
        }

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordCorrect)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Incorrect password"
          };
        }
        var userRoles = await _userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
          new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };
        foreach (var userRole in userRoles)
        {
          authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = GenerateJsonWebToken(authClaims);
        var response = new LoginResponse
        {
          Token = token,
          Username = loginDto.Username,
          Roles = userRoles
        };
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "Login successful",
          Data = response
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

    public async Task<GenericResponse> ChangePassword(string userId, PasswordReset passwordReset)
    {
      try
      {
        _logger.LogInfo("Attempting to change user password");
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "User not found",
          };
        }

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, passwordReset.OldPassword);
        if (!isPasswordCorrect)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Your old password is not correct",
          };
        }

        if (passwordReset.OldPassword == passwordReset.NewPassword)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "You cannot use your old password",
          };
        }

        var p = await _userManager.ChangePasswordAsync(user, passwordReset.OldPassword, passwordReset.NewPassword);
        if (!p.Succeeded)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Failed to change user password",
          };
        }
        await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = $"User password changed successfully",
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

    public async Task<GenericResponse> AddRoleToUser(ChangeRoleDto changeRoleDto)
    {
      if (string.IsNullOrEmpty(changeRoleDto.UserId) || string.IsNullOrEmpty(changeRoleDto.RoleName))
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "UserId or Role name is empty",
        };
      }
      try
      {
        _logger.LogInfo("Attempting to add role to user");
        var user = await _context.Users.FindAsync(changeRoleDto.UserId);
        if (user == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "User not found",
          };
        }

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == changeRoleDto.RoleName);
        if (role == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = $"Role {changeRoleDto.RoleName} not found",
          };
        }

        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);
        
        if (userRole != null)
        {
          return new GenericResponse()
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"{user.UserName} already has the role {changeRoleDto.RoleName}.",
          };
        }

        var newUserRole = new IdentityUserRole<string>
        {
          UserId = user.Id,
          RoleId = role.Id
        };

        _context.UserRoles.Add(newUserRole);
        await _context.SaveChangesAsync();

        return new GenericResponse()
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = $"Role {changeRoleDto.RoleName} added to user {user.UserName}.",
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

    public async Task<GenericResponse> RemoveRoleFromUser(ChangeRoleDto changeRoleDto)
    {
      if (string.IsNullOrEmpty(changeRoleDto.UserId) || string.IsNullOrEmpty(changeRoleDto.RoleName))
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "UserId or Role name is empty",
        };
      }

      try
      {
        _logger.LogInfo("Attempting to remove role from user");
        var user = await _context.Users.FindAsync(changeRoleDto.UserId);
        if (user == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "User not found",
          };
        }

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == changeRoleDto.RoleName);
        if (role == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = $"Role {changeRoleDto.RoleName} not found",
          };
        }

        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);
        
        if (userRole == null)
        {
          return new GenericResponse()
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = $"{user.UserName} does not have the role {changeRoleDto.RoleName}.",
          };
        }

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();

        return new GenericResponse()
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = $"Role {changeRoleDto.RoleName} removed from user {user.UserName}.",
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


    private string GenerateJsonWebToken(List<Claim> claims)
    {
      var jwt = new JwtCredentials
      {
        Secret = _configuration.GetSection("JWT:Secret").Value!,
        Issuer = _configuration.GetSection("JWT:ValidIssuer").Value!,
        Audience = _configuration.GetSection("JWT:ValidAudience").Value!,
        Lifetime = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration.GetSection("JWT:Lifetime").Value))
      };
      var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret));
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Issuer = jwt.Issuer,
        Audience = jwt.Audience,
        Subject = new ClaimsIdentity(claims),
        Expires = jwt.Lifetime,
        SigningCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }
  }
}