using System.ComponentModel.DataAnnotations;

namespace ignite_project.Models
{
  public class SignUpCode
  {
    [Key]
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsExpired => DateTime.UtcNow > CreatedAt.AddHours(7);
  }
}