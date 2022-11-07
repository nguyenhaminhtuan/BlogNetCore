namespace Api.Controllers.DTOs;

public class PaginatedDto<T>
{
    public int Count { get; set; }
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public IEnumerable<T> Items { get; set; }
}