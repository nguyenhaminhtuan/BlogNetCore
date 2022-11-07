namespace Api.Models;

public class Tag : Entity
{
    public string Slug { get; set; }
    public string Name { get; set; }
    public bool IsDeleted { get; set; } = false;
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}