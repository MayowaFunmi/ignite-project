using ignite_project.DTOs;

namespace ignite_project.Interface
{
  public interface IAuthService
  {
    Task<GenericResponse> CreateUser(RegisterDto register);
    Task<GenericResponse> AuthenticateUser(LoginDto loginDto);
    Task<GenericResponse> ChangePassword(string userId, PasswordReset passwordReset);
    Task<GenericResponse> AddRoleToUser(ChangeRoleDto changeRoleDto);
    Task<GenericResponse> RemoveRoleFromUser(ChangeRoleDto changeRoleDto);
  }
}