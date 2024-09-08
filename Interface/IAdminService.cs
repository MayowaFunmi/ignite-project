using ignite_project.DTOs;

namespace ignite_project.Interface
{
  public interface IAdminService
  {
    Task<GenericResponse> InviteCode();
    Task<PaginationResponse> GetAllUsers(int page, int pageSize);
    Task<GenericResponse> GetUserByCode(string code);
    Task<GenericResponse> GetUsersByRole(string roleName);
    Task<GenericResponse> ApproveWithdrawalRequest(ApproveRequestDto requestDto);
  }
}