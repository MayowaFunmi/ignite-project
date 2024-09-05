using ignite_project.DTOs;

namespace ignite_project.Interface
{
  public interface IAdminService
  {
    Task<GenericResponse> InviteCode();
    Task<GenericResponse> GetAllUsers();
    Task<GenericResponse> GetUserByCode(string code);
  }
}