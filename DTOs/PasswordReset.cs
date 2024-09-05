namespace ignite_project.DTOs
{
  public class PasswordReset
  {
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
  }
}