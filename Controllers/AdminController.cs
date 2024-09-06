using ignite_project.DTOs;
using ignite_project.Interface;
using ignite_project.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ignite_project.Controllers
{
  public class AdminController(IAdminService adminService) : BaseController
  {
    private readonly IAdminService _adminService = adminService;

    #region InvitationCode
    /// <summary>
    /// Generate invitation code for signup
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    [HttpPost]
    [Route("generate-invitation-code")]
    [Authorize(Policy = "AdminSuperAdmin")]

    public async Task<IActionResult> InvitationCode()
    {
      try
      {
        var response = await _adminService.InviteCode();
        return response.Status switch
        {
          "BadRequest" => BadRequest(response),
          _=> Ok(response)
        };
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }
    #endregion

    #region Get All users
    /// <summary>
    /// Get all users
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    [HttpGet]
    [Route("show-all-users")]
    [Authorize(Policy = "AdminSuperAdmin")]

    public async Task<IActionResult> AllUsers([FromQuery] PageRequest pageRequest)
    {
      try
      {
        var response = await _adminService.GetAllUsers(pageRequest.Page, pageRequest.PageSize);
        return response.Status switch
        {
          "BadRequest" => BadRequest(response),
          _=> Ok(response)
        };
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }
    #endregion
    
    #region InvitationCode
    /// <summary>
    /// Get user by user invite code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("get-user-by-code")]
    [Authorize(Policy = "SuperAdmin")]

    public async Task<IActionResult> UserByCode([FromQuery] string code)
    {
      try
      {
        var response = await _adminService.GetUserByCode(code);
        return response.Status switch
        {
          "BadRequest" => BadRequest(response),
          "NotFound" => NotFound(response),
          _=> Ok(response)
        };
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }
    #endregion

    #region Get Users By Role Name
    /// <summary>
    /// Get users by role
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("get-users-by-role")]
    [Authorize(Policy = "SuperAdmin")]

    public async Task<IActionResult> UsersByRole([FromQuery] string roleName)
    {
      try
      {
        var response = await _adminService.GetUsersByRole(roleName);
        return response.Status switch
        {
          "BadRequest" => BadRequest(response),
          "NotFound" => NotFound(response),
          _=> Ok(response)
        };
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }

    #endregion
  }
}