using ignite_project.DTOs;

namespace ignite_project.Interface
{
  public interface IUserService
  {
    Task<GenericResponse> GetUserById(string userId);
    Task<GenericResponse> AddUserWalletAddress(AddWalletDto addWalletDto);
    Task<GenericResponse> AddUserProfilePicture(AddProfilePictureDto pictureDto);
    Task<GenericResponse> UserWithdrawal(WithdrawalRequestDto requestDto);
  }
}