namespace ignite_project.DTOs
{
  public class RegisterDto
  {
    public string Username { get; set; } = string.Empty;
    public string InvitationCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
  }
}