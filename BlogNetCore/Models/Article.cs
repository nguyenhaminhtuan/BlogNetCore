namespace BlogNetCore.Models;

public class Article
{
    public int Id { get; set; }
    public string Slug { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public ArticleStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public int AuthorId { get; set; }
    public User Author { get; set; }
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

public enum ArticleStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2,
    Deleted = 3
}