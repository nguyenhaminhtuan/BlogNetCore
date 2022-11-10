namespace Api.Models;

public class Vote : Entity
{
    public bool IsPositive { get; set; }
    public DateTime VotedAt { get; set; } = DateTime.UtcNow;
    public int OwnerId { get; set; }
    public int? ArticleId { get; set; }
    public int? CommentId { get; set; }
    public User Owner { get; set; }
    public Article? Article { get; set; }
    public Comment? Comment { get; set; }
}