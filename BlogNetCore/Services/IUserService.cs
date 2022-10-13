using BlogNetCore.Models;
using BlogNetCore.Common.Exceptions;
using OneOf;

namespace BlogNetCore.Services;

public interface IUserService
{
    bool IsValidCredentials(User user, string password);
    Task<User?> GetUserByUsername(string username);
    Task<OneOf<User, EmailAlreadyExists>> CreateUser(string username, string password, string displayName);
}