namespace Api.Models;

public class PaginateParams
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}