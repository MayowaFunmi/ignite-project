using ignite_project.DTOs;

namespace ignite_project.Interface
{
  public interface IUserService
  {
    Task<GenericResponse> GetUserById(string userId);
    Task<GenericResponse> GetUserByRole(string roleName);
    Task<GenericResponse> AddUserProfile(string userId, UserProfileDto userProfileDto);
  }
}