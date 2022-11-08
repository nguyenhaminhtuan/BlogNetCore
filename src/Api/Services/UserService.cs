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
    private readonly IEmailService _emailService;

    public UserService(
        ILogger<UserService> logger,
        IPasswordHasher<User> passwordHasher,
        ApplicationDbContext db,
        IDataProtectionProvider dataProtectionProvider,
        IEmailService emailService)
    {
        _logger = logger;
        _passwordHasher = passwordHasher;
        _db = db;
        _verifyProtector = dataProtectionProvider
            .CreateProtector("VerifyEmail")
            .ToTimeLimitedDataProtector();
        _emailService = emailService;
    }

    public bool IsValidCredentials(User user, string password)
    {
        return _passwordHasher.VerifyHashedPassword(user, user.Password, password) ==
               PasswordVerificationResult.Success;
    }
    
    public Task<User?> GetUserById(int id)
    {
        return _db.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public Task<User?> GetUserByUsername(string username)
    {
        return _db.Users
            .FirstOrDefaultAsync(u => string.Equals(u.Username, username));
    }
    
    private async Task<bool> ExistsByUsername(string username)
    {
        return (await GetUserByUsername(username)) is not null;
    }

    public Task<User?> GetUserByProfileName(string profileName)
    {
        return _db.Users
            .Where(u => string.Equals(u.ProfileName, profileName))
            .FirstOrDefaultAsync();
    }

    public async Task RegisterUser(string username, string password)
    {
        if (await ExistsByUsername(username))
            throw new ConflictException("Username given already exists");

        var guid = Guid.NewGuid().ToString();
        var user = new User
        {
            Username = username,
            ProfileName = $"user_{guid.Replace("-", "_")}",
            DisplayName = $"User {guid.Replace("-", " ")}",
            Status = UserStatus.Active
        };
        user.Password = _passwordHasher.HashPassword(user, password);
        await _db.AddAsync(user);
        await _db.SaveChangesAsync();
        await SendVerifyEmail(user.Username, user.Id);
        _logger.LogInformation("User {Username} registered", user.Username);
    }

    public async Task UpdateUserProfile(User user, string profileName, string displayName)
    {
        var existedUser = await _db.Users
            .FirstOrDefaultAsync(u => u.Id != user.Id && string.Equals(u.ProfileName, profileName));
        if (existedUser is not null)
            throw new ConflictException("Profile name given already exists");

        user.ProfileName = profileName;
        user.DisplayName = displayName;
        await _db.SaveChangesAsync();
        _logger.LogInformation("User {Username} updated profile {ProfileName}",
            user.Username, user.ProfileName);
    }

    public async Task SendVerifyEmail(string to, int userId)
    {
        var content = _verifyProtector.Protect(userId.ToString(), TimeSpan.FromMinutes(10));
        await _emailService.Send("no-reply@blognetcore.com", to, "[BlogNetCore] Verify email", content);
        _logger.LogInformation("Verify email was sent to {Email}", to);
    }

    public async Task<User?> GetUserFromVerifyCode(string verifyCode)
    {
        var userIdString = _verifyProtector.Unprotect(verifyCode);
        if (!int.TryParse(userIdString, out var userId))
            return null;
            
        return await GetUserById(userId);
    }

    public async Task VerifyUser(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        
        if (!user.EmailVerified)
        {
            user.EmailVerified = true;
            user.Status = UserStatus.Active;
            user.LastChanged = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            _logger.LogInformation("User {UserId} verified at {VerifyTime}", user.Id, DateTime.UtcNow);
        }
    }
}