namespace Api.Models;

public class User : Entity
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string ProfileName { get; set; }
    public string DisplayName { get; set; }
    public bool EmailVerified { get; set; }
    public UserStatus Status { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastChanged { get; set; } = DateTime.UtcNow;
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}

public enum UserStatus : byte
{
    Active = 0,
    Disabled = 1
}

public enum UserRole : byte
{
    User = 0,
    Admin = 1
}