using ignite_project.Constants;

namespace ignite_project.DTOs
{
  public class WithdrawalRequestDto
  {
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string WalletAddress { get; set; } = string.Empty;
  }
}