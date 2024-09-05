using ignite_project.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ignite_project.Controllers
{
  public class UserController(IUserService userService) : BaseController
  {
    private readonly IUserService _userService = userService;

    #region InvitationCode
    /// <summary>
    /// Get user bu user id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("get-user-by-id/{userId}")]
    [Authorize]

    public async Task<IActionResult> UserById(string userId)
    {
      try
      {
        var response = await _userService.GetUserById(userId);
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