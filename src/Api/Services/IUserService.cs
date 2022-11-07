using Api.Models;

namespace Api.Services;

public interface IUserService
{
    public bool IsValidCredentials(User user, string password);
    public Task<User?> GetUserById(int id);
    public Task<User?> GetUserByUsername(string username);
    public Task<User?> GetUserProfile(string profileName);
    public Task RegisterUser(string username, string password);
    public Task UpdateUserProfile(User user, string profileName, string displayName);
    public Task SendVerifyEmail(string to, int userId);
    public Task<bool> VerifyUserEmail(string code);
}