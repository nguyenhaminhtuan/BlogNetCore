using System.Security.Claims;
using Api.Auth;
using Api.Controllers.DTOs;
using Api.Extensions;
using Api.Models;
using Api.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class AccountController : ApiControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IUserService _userService;
    
    public AccountController(
        ILogger<AccountController> logger,
        IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }
    
    [Route("unauthorized")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult RedirectLogin()
    {
        return Unauthorized("Authentication required");
    }
    
    [Route("forbidden")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult RedirectToAccessDenied()
    {
        return Forbid("You do not have permission for access");
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        UserCredentialsDto dto,
        [FromServices] IValidator<UserCredentialsDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        validationResult.AddToModelState(ModelState);
        if (!validationResult.IsValid)
            return BadRequest(ModelState);

        var user = await _userService.GetUserByUsername(dto.Username);
        if (user is null || !_userService.IsValidCredentials(user, dto.Password))
            return BadRequest(
                title: "Bad credentials",
                detail: "Invalid username or password");

        if (user.Status == UserStatus.Disabled)
            return Forbid("Your account has been disabled");

        var claims = new List<Claim>
        {
            new(CookieClaimTypes.Identity, user.Id.ToString()),
            new(CookieClaimTypes.Username, user.Username),
            new(CookieClaimTypes.Role, user.Role.ToString()),
            new(CookieClaimTypes.EmailVerified, (user.EmailVerified).ToString()),
            new(CookieClaimTypes.IsDisabled, (user.Status == UserStatus.Disabled).ToString()),
            new(CookieClaimTypes.LastChanged, user.LastChanged.ToString("o"))
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
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        UserCredentialsDto dto,
        [FromServices] IValidator<UserCredentialsDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        validationResult.AddToModelState(ModelState);
        if (!validationResult.IsValid)
            return BadRequest(ModelState);
        
        await _userService.RegisterUser(dto.Username, dto.Password);
        return Ok();
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.GetUserId();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("User {UserId} logged out", userId);
        return Ok();
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailDto dto)
    {
        var user = await _userService.GetUserFromVerifyCode(dto.VerifyCode);
        if (user is null)
            return BadRequest("Invalid or expired verify code");

        await _userService.VerifyUser(user);
        return Ok();
    }

    [Authorize(Policy = AuthorizationPolicies.ActiveUserOnly)]
    [HttpPost("resend-verify")]
    public async Task<IActionResult> ResendVerifyEmail()
    {
        if (User.HasClaim(CookieClaimTypes.EmailVerified, bool.TrueString))
            return Forbid("Your account already verified");
        
        var emailClaim = User.FindFirst(c => c.Type == CookieClaimTypes.Username);
        await _userService.SendVerifyEmail(emailClaim!.Value, User.GetUserId());
        return Ok();
    }
}