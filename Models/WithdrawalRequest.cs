using System.ComponentModel.DataAnnotations;
using ignite_project.Constants;

namespace ignite_project.Models
{
  public class WithdrawalRequest
  {
    [Key]
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string WalletAddress { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime ApprovedAt { get; set; }
    public WithdrawalStatus Status { get; set; } = WithdrawalStatus.UNREQUESTED;
  }
}