namespace ignite_project.DTOs
{
  public class UserProfileDto
  {
    public string WalletAddress { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public string WalletBalance { get; set; } = string.Empty;
    public int Ratings { get; set; }
  }
}