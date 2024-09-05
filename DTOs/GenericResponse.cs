namespace ignite_project.DTOs
{
  public class GenericResponse
  {
    public string? Status { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
  }
}