namespace ignite_project.DTOs
{
  public class PaginationMetaData
  {
    public PaginationMetaData(int page, int pageSize, int totalItems)
    {
      CurrentPage = page == 0 ? 1 : page;
      NextPage = HasNextPage ? CurrentPage + 1 : 0;
      PreviousPage = HasPreviousPage ? CurrentPage - 1 : 0;
      PageSize = pageSize < 1 || pageSize > 50 ? 20 : pageSize;
      PageCount = (int)Math.Ceiling(totalItems / (double)PageSize);
      TotalRecords = totalItems;
    }

    public int CurrentPage { get; set; }
    public int NextPage { get; set; }
    public int PreviousPage { get; set; }
    public bool HasNextPage { get { return CurrentPage < PageCount; } }
    public bool HasPreviousPage { get { return CurrentPage > 1; } }
    public int PageCount { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
  }
}