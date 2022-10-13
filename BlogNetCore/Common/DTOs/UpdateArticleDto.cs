using System.ComponentModel.DataAnnotations;

namespace BlogNetCore.Common.DTOs;

public class UpdateArticleDto
{
    [MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [MinLength(1)]
    public string Content { get; set; } = string.Empty;
    
    public List<int> TagIds { get; set; } = new();
}