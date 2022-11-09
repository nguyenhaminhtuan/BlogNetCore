using Api.Models;

namespace Api.Services;

public interface IUserService
{
    public bool IsValidCredentials(User user, string password);
    public Task<User?> GetUserById(int id);
    public Task<User?> GetUserByUsername(string username);
    public Task<User?> GetUserByProfileName(string profileName);
    public Task RegisterUser(string username, string password);
    public Task UpdateUser(User user, string profileName, string displayName);
    public Task SendVerifyEmail(string to, int userId);
    public Task<User?> GetUserFromVerifyCode(string verifyCode);
    public Task VerifyUser(User user);
}