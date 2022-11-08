namespace Api.Controllers.DTOs;

public class ProfileDto
{
   public int Id { get; set; }
   public string ProfileName { get; set; } = string.Empty;
   public string DisplayName { get; set; } = string.Empty;
   public bool IsDisabled { get; set; }
   public int TotalArticles { get; set; }
}