using System.Net;
using ignite_project.Constants;
using ignite_project.Data;
using ignite_project.DTOs;
using ignite_project.Interface;
using Microsoft.EntityFrameworkCore;
using WatchDog;

namespace ignite_project.Implementation
{
  public class AccountService(ILoggerManager logger, ApplicationDbContext context) : IAccountService
  {
    private readonly ILoggerManager _logger = logger;
    private readonly ApplicationDbContext _context = context;

    public async Task<PaginationResponse> GetAllWithdrawalRequests(int page, int pageSize)
    {
      try
      {
        _logger.LogInfo("Attempting to get all users withdrawal requests");
        var requests = await _context.WithdrawalRequests
                          .OrderByDescending(u => u.RequestedAt)
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .AsNoTracking()
                          .ToListAsync();
        var paginationMetaData = new PaginationMetaData(page, pageSize, requests.Count);
        return new PaginationResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "All withdrawal requests retrieved successfully",
          Data = requests,
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

    public async Task<GenericResponse> GetWithdrawalRequestById(string requestId)
    {
      if (string.IsNullOrEmpty(requestId))
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "Withdrawal request id is required",
        };
      }
      
      try
      {
        _logger.LogInfo($"Attempting to get withrawal request by request id - {requestId}");
        var request = await _context.WithdrawalRequests.AsNoTracking().FirstOrDefaultAsync(w => w.Id.ToString() == requestId);
        if (request == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "Withdrawal request not found",
          };
        }
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "Withdrawal request retrieved successfully",
          Data = request
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

    public async Task<GenericResponse> GetWithdrawalRequestByStatus(string status)
    {
      if (string.IsNullOrEmpty(status))
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "withdrawal request status is required",
        };
      }
      try
      {
        _logger.LogInfo($"Attempting to get withrawal requests by status - {status}");
        if (!Enum.TryParse(typeof(WithdrawalStatus), status, true, out var parsedStatus))
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Invalid withdrawal request status",
          };
        }

        var withdrawalStatus = (WithdrawalStatus)parsedStatus;
        var requests = await _context.WithdrawalRequests
                      .Where(w => w.Status == withdrawalStatus)
                      .ToListAsync();

        if (requests == null || requests.Count == 0)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "No withdrawal request found for the given status",
          };
        }
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "Withdrawal requests retrieved successfully",
          Data = requests
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

    public async Task<GenericResponse> GetWithdrawalRequestByUserId(string userId)
    {
      if (string.IsNullOrEmpty(userId))
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "User id is required",
        };
      }
      try
      {
        _logger.LogInfo($"Attempting to get withrawal requests for a user with id - {userId}");
        var requests = await _context.WithdrawalRequests.AsNoTracking().Where(w => w.UserId == userId).ToListAsync();
        
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "Withdrawal requests retrieved successfully",
          Data = requests
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