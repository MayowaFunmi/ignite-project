using ignite_project.DTOs;

namespace ignite_project.Interface
{
  public interface IAccountService
  {
    Task<PaginationResponse> GetAllWithdrawalRequests(int page, int pageSize);
    Task<GenericResponse> GetWithdrawalRequestById(string requestId);
    Task<GenericResponse> GetWithdrawalRequestByUserId(string userId);
    Task<GenericResponse> GetWithdrawalRequestByStatus(string status);
  }
}