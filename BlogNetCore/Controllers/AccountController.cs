using System.Net;
using System.Security.Claims;
using BlogNetCore.Common.DTOs;
using BlogNetCore.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogNetCore.Controllers;

public class AccountController : ApiControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IUserService _userService;
    
    public AccountController(ILogger<AccountController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserCredentialsDto request)
    {
        var user = await _userService.GetUserByUsername(request.Username);
        const string badCredTitle = "Bad credentials";
        if (user is null)
            return ValidationProblem(title: badCredTitle);

        if (!_userService.IsValidCredentials(user, request.Password))
            return ValidationProblem(title: badCredTitle);

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
        _logger.LogInformation("User {Username} logged in", user.Username);

        return Ok();
    }

    [AllowAnonymous]
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
        _logger.LogInformation("User {Username} logged out", usernameClaim?.Value);
        return Ok();
    }
}