namespace ignite_project.DTOs
{
  public class UserDetails
  {
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string InvitationCode { get; set; } = string.Empty;
    public string WalletAddress { get; set; } = string.Empty;
    public string CryptoCoin { get; set; } = string.Empty;
    public decimal WalletBallance { get; set; }
    public string Location { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public bool ActiveSubscription { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public decimal Ratings { get; set; } = 0;
    public int WithdrawalRequests { get; set; } = 0;
    public decimal TotalWithdrawals { get; set; } = 0;
    public string SignupCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
  }
}