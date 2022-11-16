namespace Api.Controllers.DTOs;

public class CreateReplyDto
{
    public string Body { get; set; }
    public int? ReplyToId { get; set; }
}