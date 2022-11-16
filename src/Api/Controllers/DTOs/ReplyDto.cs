namespace Api.Controllers.DTOs;

public class ReplyDto
{
    public int Id { get; set; }
    public string Body { get; set; }
    public DateTime CommentedAt { get; set; }
    public UserDto Owner { get; set; }
    public UserDto ReplyTo { get; set; }
    public int UpVoteCount { get; set; }
    public int DownVoteCount { get; set; }
}