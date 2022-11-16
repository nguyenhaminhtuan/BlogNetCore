namespace Api.Controllers.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string ProfileName { get; set; }
    public string DisplayName { get; set; }
    public bool IsDisabled { get; set; }
}