using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models;

public class Article : Entity
{
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
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();

    [NotMapped]
    public int TotalUpVotes { get; set; }

    [NotMapped]
    public int TotalDownVotes { get; set; }
    
    [NotMapped]
    public int TotalComments { get; set; }
}

public enum ArticleStatus : byte
{
    Draft = 0,
    Published = 1,
    Archived = 2,
    Deleted = 3
}