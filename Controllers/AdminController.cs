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
    //[Authorize(Policy = "AdminSuperAdmin")]

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
    
  }
}