using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models;

public class Comment : Entity
{
    public string Body { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CommentedAt { get; set; } = DateTime.UtcNow;
    public int OwnerId { get; set; }
    public int ArticleId { get; set; }
    public int? ReplyFromId { get; set; }
    public int? ReplyToId { get; set; }
    public User Owner { get; set; }
    public Article Article { get; set; }
    public User? ReplyTo { get; set; }
    public Comment? ReplyFrom { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();

    [NotMapped]
    public int UpVoteCount { get; set; }

    [NotMapped]
    public int DownVoteCount { get; set; }
}