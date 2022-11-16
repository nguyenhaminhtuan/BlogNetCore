namespace Api.Models;

public class PaginateQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}