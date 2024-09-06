namespace ignite_project.DTOs
{
  public class PaginationResponse
  {
    public string? Status { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
    public PaginationMetaData? Pagination {get; set; }
  }
}