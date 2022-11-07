namespace Api.Models;

public class PaginateQueryParams
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}