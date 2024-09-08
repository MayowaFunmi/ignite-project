using ignite_project.DTOs;
using ignite_project.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ignite_project.Controllers
{
  public class UserController(IUserService userService) : BaseController
  {
    private readonly IUserService _userService = userService;

    #region Get User By Id
    /// <summary>
    /// Get user by user id
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

    #region Add User Wallet Address
    /// <summary>
    /// Add User Wallet Address
    /// </summary>
    /// <param name="addWalletDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("add-user-water-address")]
    [Authorize]

    public async Task<IActionResult> AddWalletAddress([FromBody] AddWalletDto addWalletDto)
    {
      try
      {
        var response = await _userService.AddUserWalletAddress(addWalletDto);
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

    #region Add User Profile Picture
    /// <summary>
    /// Add User Profile Picture
    /// </summary>
    /// <param name="pictureDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("add-user-profile-picture")]
    [Authorize]

    public async Task<IActionResult> AddProfilePicture([FromForm] AddProfilePictureDto pictureDto)
    {
      try
      {
        var response = await _userService.AddUserProfilePicture(pictureDto);
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

    #region Submit withdrawal request
    /// <summary>
    /// Submit withdrawal request
    /// </summary>
    /// <param name="requestDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("submit-withdrawal-request")]
    [Authorize]

    public async Task<IActionResult> SubmitWithdrawalRequest([FromBody] WithdrawalRequestDto requestDto)
    {
      try
      {
        var response = await _userService.UserWithdrawal(requestDto);
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