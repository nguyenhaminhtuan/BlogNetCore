using BlogNetCore.Data;
using BlogNetCore.Models;
using BlogNetCore.Common.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace BlogNetCore.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ApplicationDbContext _db;

    public UserService(
        ILogger<UserService> logger,
        IPasswordHasher<User> passwordHasher,
        ApplicationDbContext db)
    {
        _logger = logger;
        _passwordHasher = passwordHasher;
        _db = db;
    }

    public bool IsValidCredentials(User user, string password)
    {
        return _passwordHasher.VerifyHashedPassword(user, user.Password, password) ==
               PasswordVerificationResult.Success;
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => string.Equals(u.Username, username));
    }

    public async Task<bool> ExistsByUsername(string username)
    {
        return (await GetUserByUsername(username)) is not null;
    }

    public async Task<OneOf<User, EmailAlreadyExists>> CreateUser(string username, string password, string displayName)
    {
        if (await ExistsByUsername(username))
            return new EmailAlreadyExists();

        var user = new User
        {
            Username = username,
            DisplayName = displayName
        };
        user.Password = _passwordHasher.HashPassword(user, password);

        await _db.AddAsync(user);
        await _db.SaveChangesAsync();
        _logger.LogInformation("User {Username} registered", user.Username);

        return user;
    }
}