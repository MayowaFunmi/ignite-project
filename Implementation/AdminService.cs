using System.Net;
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
  }
}