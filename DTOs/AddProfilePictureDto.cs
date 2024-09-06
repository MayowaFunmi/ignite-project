namespace ignite_project.DTOs
{
  public class AddProfilePictureDto
  {
    public string UserId { get; set; } = string.Empty;
    public required IFormFile File { get; set; }
  }
}