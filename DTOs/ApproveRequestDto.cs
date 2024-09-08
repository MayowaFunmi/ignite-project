namespace ignite_project.DTOs
{
  public class ApproveRequestDto
  {
    public string UserId { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
    public decimal Amount { get; set; } = 0;
    public bool Status { get; set; } = false;
  }
}