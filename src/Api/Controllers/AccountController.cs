using System.Security.Claims;
using Api.Controllers.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

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
    public async Task<IActionResult> Login(UserCredentialsDto request)
    {
        var user = await _userService.GetUserByUsernameAsync(request.Username);
        if (user is null || !_userService.IsValidCredentials(user, request.Password))
            return BadRequest(
                title: "Bad credentials.",
                detail: "Invalid username or password.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
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
    public async Task<IActionResult> Register(UserCredentialsDto request)
    {
        await _userService.RegisterUserAsync(request.Username, request.Password);
        return Ok();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var usernameClaim = HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Name);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("User {Username} logged out", usernameClaim?.Value);
        return Ok();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [Route("unauthorized")]
    public IActionResult RedirectLogin()
    {
        return Unauthorized();
    }
    
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [Route("forbidden")]
    public IActionResult RedirectToAccessDenied()
    {
        return Forbid();
    }
}