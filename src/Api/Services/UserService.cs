using Api.Data;
using Api.Exceptions;
using Api.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ApplicationDbContext _db;
    private readonly ITimeLimitedDataProtector _verifyProtector;

    public UserService(
        ILogger<UserService> logger,
        IPasswordHasher<User> passwordHasher,
        ApplicationDbContext db,
        IDataProtectionProvider dataProtectionProvider)
    {
        _logger = logger;
        _passwordHasher = passwordHasher;
        _db = db;
        _verifyProtector = dataProtectionProvider
            .CreateProtector("VerifyEmail")
            .ToTimeLimitedDataProtector();
    }

    public bool IsValidCredentials(User user, string password)
    {
        return _passwordHasher.VerifyHashedPassword(user, user.Password, password) ==
               PasswordVerificationResult.Success;
    }
    
    public Task<User?> GetUserByIdAsync(int id)
    {
        return _db.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public Task<User?> GetUserByUsernameAsync(string username)
    {
        return _db.Users
            .FirstOrDefaultAsync(u => string.Equals(u.Username, username));
    }

    public Task<User?> GetUserProfileAsync(string profileName)
    {
        return _db.Users
            .Include(u => u.Articles)
            .Where(u => string.Equals(u.ProfileName, profileName))
            .Where(u => u.Status != UserStatus.Verifying)
            .FirstOrDefaultAsync();
    }

    public async Task RegisterUserAsync(string username, string password)
    {
        if (await ExistsByUsername(username))
            throw new ConflictException("Username given already exists.");

        var guid = new Guid();
        var user = new User
        {
            Username = username,
            ProfileName = $"user-{guid}",
            DisplayName = $"User {guid}",
            Status = UserStatus.Verifying
        };
        user.Password = _passwordHasher.HashPassword(user, password);
        await _db.AddAsync(user);
        await _db.SaveChangesAsync();
        _logger.LogInformation("User {Username} registered", user.Username);
    }

    public async Task CreateUserProfileAsync(User user, string profileName, string displayName)
    {
        var existedUser = await _db.Users
            .FirstOrDefaultAsync(u => string.Equals(u.ProfileName, profileName));
        if (existedUser is not null)
            throw new ConflictException("Profile name given already exists.");

        user.ProfileName = profileName;
        user.DisplayName = displayName;
        await _db.SaveChangesAsync();
        _logger.LogInformation("User {Username} create new profile {ProfileName}",
            user.Username, user.ProfileName);
    }

    public string CreateVerifyHash(User user)
    {
        return _verifyProtector.Protect(user.Id.ToString(), TimeSpan.FromMinutes(10));
    }

    private async Task<bool> ExistsByUsername(string username)
    {
        return (await GetUserByUsernameAsync(username)) is not null;
    }
}