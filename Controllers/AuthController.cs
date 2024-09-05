using System.Net;
using ignite_project.DTOs;
using ignite_project.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ignite_project.Controllers
{
  public class AuthController(IAuthService userService) : BaseController
  {
    private readonly IAuthService _userService = userService;

    #region CreateUser
    /// <summary>
    /// User sign up registration
    /// </summary>
    /// <param name="registerDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("register-user")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterDto registerDto)
    {
      try
      {
        var response = await _userService.CreateUser(registerDto);
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

    #region Login User
    /// <summary>
    /// User login
    /// </summary>
    /// <param name="loginDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("login")]

    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
      if (!string.IsNullOrEmpty(CurrentUserId))
        return Unauthorized("current user is already logged in");

      try
      {
        var response = await _userService.AuthenticateUser(loginDto);
        if (response.Status == HttpStatusCode.NotFound.ToString() || response.Status == HttpStatusCode.BadRequest.ToString())
          return Unauthorized(response.Message);

        Response.Headers.Authorization = "Bearer " + response.Data;
        return Ok(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }
    #endregion

    #region Change User Password
    /// <summary>
    /// change a user password
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="passwordReset"></param
    /// <returns></returns>
    [HttpPut]
    [Route("change-user-password/{userId}")]
    [Authorize]

    public async Task<IActionResult> ChangeUserPassword(string userId, [FromBody] PasswordReset passwordReset)
    {
      try
      {
        if (CurrentUserId != userId)
          return BadRequest("you cannot update another user's password");
        var response = await _userService.ChangePassword(userId, passwordReset);
        return response.Status == HttpStatusCode.BadRequest.ToString() ? BadRequest(response) : Ok(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred while processing your request - {ex.Message}");
      }
    }
    #endregion

    #region Add Role To User
    /// <summary>
    /// Add more roles to user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleName"></param
    /// <returns></returns>
    [HttpPost]
    [Route("add-role-to-user")]
    [Authorize(Policy = "SuperAdmin")]

    public async Task<IActionResult> AddRoleToUser([FromBody] ChangeRoleDto changeRoleDto)
    {
      try
      {
        var response = await _userService.AddRoleToUser(changeRoleDto);
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

    #region Rmove Role From User
    /// <summary>
    /// Remove role from user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleName"></param
    /// <returns></returns>
    [HttpDelete]
    [Route("remove-user-from-role")]
    [Authorize(Policy = "SuperAdmin")]

    public async Task<IActionResult> RemoveRoleFromUser([FromBody] ChangeRoleDto changeRoleDto)
    {
      try
      {
        var response = await _userService.RemoveRoleFromUser(changeRoleDto);
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