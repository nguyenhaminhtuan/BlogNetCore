using Api.Models;

namespace Api.Services;

public interface IUserService
{
    public bool IsValidCredentials(User user, string password);
    public Task<User?> GetUserByIdAsync(int id);
    public Task<User?> GetUserByUsernameAsync(string username);
    public Task<User?> GetUserProfileAsync(string profileName);
    public Task RegisterUserAsync(string username, string password);
    public Task CreateUserProfileAsync(User user, string profileName, string displayName);
    public Task SendVerifyEmail(string to, int userId);
    public Task<bool> VerifyUserEmail(string code);
}