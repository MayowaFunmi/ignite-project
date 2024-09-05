using System.Net;
using ignite_project.Data;
using ignite_project.DTOs;
using ignite_project.Interface;
using ignite_project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WatchDog;

namespace ignite_project.Implementation
{
  public class UserService(UserManager<ApplicationUser> userManager, ILoggerManager logger, IConfiguration configuration, ApplicationDbContext context) : IUserService
  {
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILoggerManager _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly ApplicationDbContext _context = context;

    public Task<GenericResponse> AddUserProfile(string userId, UserProfileDto userProfileDto)
    {
        throw new NotImplementedException();
    }

    public Task<GenericResponse> GetUserByRole(string roleName)
    {
        throw new NotImplementedException();
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
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
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
  }
}