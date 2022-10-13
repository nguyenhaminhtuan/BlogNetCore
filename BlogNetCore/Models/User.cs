using System.Text.Json.Serialization;

namespace BlogNetCore.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string DisplayName { get; set; }
    public UserStatus Status { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastChanged { get; set; } = DateTime.UtcNow;
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}

public enum UserStatus
{
    Verifying = 0,
    Active = 1,
    Disabled = 2
}

public enum UserRole
{
    User = 0,
    Admin = 1
}