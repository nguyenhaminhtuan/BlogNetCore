namespace Api.Controllers.DTOs;

public class CreateArticleDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public HashSet<int> TagIds { get; set; } = new();
}