using ignite_project.DTOs;
using ignite_project.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ignite_project.Controllers
{
  public class AccountController(IAccountService accountService) : BaseController
  {
    private readonly IAccountService _accountService = accountService;

    #region Get All Withdrawal Requests
    /// <summary>
    /// Get All Withdrawal Requests
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("get-all-withdrawal-requests")]
    [Authorize(Policy = "SuperAdmin")]

    public async Task<IActionResult> AllWithdrawalRequest([FromQuery] PageRequest pageRequest)
    {
      try
      {
        var response = await _accountService.GetAllWithdrawalRequests(pageRequest.Page, pageRequest.PageSize);
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

    #region Get Withdrawal Requests By Id
    /// <summary>
    /// Get Withdrawal Requests By Id
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("get-withdrawal-request-by-id/{requestId}")]
    [Authorize(Policy = "SuperAdmin")]

    public async Task<IActionResult> WithdrawalRequestById(string requestId)
    {
      try
      {
        var response = await _accountService.GetWithdrawalRequestById(requestId);
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

    #region Get Withdrawal Requests By User Id
    /// <summary>
    /// Get Withdrawal Requests By User Id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("get-withdrawal-requests-by-user-id/{userId}")]
    [Authorize(Policy = "SuperAdmin")]

    public async Task<IActionResult> WithdrawalRequestByUserId(string userId)
    {
      try
      {
        var response = await _accountService.GetWithdrawalRequestByUserId(userId);
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

    #region Get Withdrawal Requests By Status
    /// <summary>
    /// Get Withdrawal Requests By Status
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("get-withdrawal-requests-by-status")]
    [Authorize(Policy = "SuperAdmin")]

    public async Task<IActionResult> WithdrawalRequestByStatus(string status)
    {
      try
      {
        var response = await _accountService.GetWithdrawalRequestByStatus(status);
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