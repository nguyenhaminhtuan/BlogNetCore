namespace Api.Controllers.DTOs;

public class ProfileDto
{
   public int Id { get; set; }
   public string Username { get; set; } = string.Empty;
   public string DisplayName { get; set; } = string.Empty;
   public bool IsVerified { get; set; }
   public ICollection<ArticleDto> Articles { get; set; } = new List<ArticleDto>();
}