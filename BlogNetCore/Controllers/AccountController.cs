using System.Net;
using System.Security.Claims;
using BlogNetCore.Common.DTOs;
using BlogNetCore.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace BlogNetCore.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IUserService _userService;
    
    public AccountController(ILogger<AccountController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserCredentialsDto request)
    {
        var user = await _userService.GetUserByUsername(request.Username);
        if (user is null)
            return Problem(
                title: "User not found.",
                detail: $"Can't find user with username {request.Username}",
                statusCode: (int)HttpStatusCode.NotFound);

        if (!_userService.IsValidCredentials(user, request.Password))
            return Problem(
                title: "Bad credentials",
                statusCode: (int)HttpStatusCode.BadRequest);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Id.ToString()),
            new(ClaimTypes.Email, user.Username),
            new(ClaimTypes.Role, user.Role.ToString())
        };
        var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
            });
        _logger.LogInformation(
            "User {Username} logged in at {LoginTime}",
            user.Username, DateTimeOffset.UtcNow.ToString("u"));

        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto request)
    {
        var result = await _userService.CreateUser(
            request.Username,
            request.Password,
            request.DisplayName);
        return result.Match<IActionResult>(
            _ => Ok(),
            emailExisted => Problem(
                title: emailExisted.Message,
                detail: $"Email {request.Username} has been taken, please use another email.",
                statusCode: (int)HttpStatusCode.Conflict));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var usernameClaim = HttpContext.User.FindFirst((claims) => claims.Type == ClaimTypes.Email);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation(
            "User {Username} logged out at {LogoutTime}",
            usernameClaim?.Value, DateTime.UtcNow.ToString("u"));
        return Ok();
    }
}