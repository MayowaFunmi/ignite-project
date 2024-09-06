using Microsoft.AspNetCore.Identity;

namespace ignite_project.Models
{
  public class ApplicationUser : IdentityUser
  {
    public string InvitationCode { get; set; } = string.Empty;
    public string WalletAddress { get; set; } = string.Empty;
    public string CryptoCoin { get; set; } = string.Empty;
    public int WalletBallance { get; set; }
    public string Location { get; set; } = string.Empty;
    public bool ActiveSubscription { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int Ratings { get; set; } = 0;
    public string SignupCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    public ApplicationUser()
    {
      PhoneNumberConfirmed = true;
      EmailConfirmed = true;
    }
  }
}