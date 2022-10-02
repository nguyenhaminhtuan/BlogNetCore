namespace BlogNetCore.Models;

public class Article
{
    public int Id { get; set; }
    public string Slug { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public PostStatus PostStatus { get; set; } = PostStatus.Draft;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset LastModifiedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? PublishedAt { get; set; }
    public User Author { get; set; }
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

public enum PostStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2,
    Deleted = 3
}