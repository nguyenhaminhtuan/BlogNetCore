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
using Swashbuckle.AspNetCore.Annotations;

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
    [SwaggerOperation(Summary = "Login use username and password")]
    [SwaggerResponse(StatusCodes.Status200OK, "Logged in successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data or bad credentials")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Account disabled")]
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
            new(ClaimTypes.Name, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(ClaimTypes.Email, user.Username),
            new(AdditionalClaimTypes.EmailVerified, (user.EmailVerified).ToString()),
            new(AdditionalClaimTypes.IsDisabled, (user.Status == UserStatus.Disabled).ToString()),
            new(AdditionalClaimTypes.LastChanged, user.LastChanged.ToString("o"))
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
    [SwaggerOperation(Summary = "Register account use username and password")]
    [SwaggerResponse(StatusCodes.Status200OK, "Account registered successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid register account data")]
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
    
    [HttpPost("logout")]
    [Authorize]
    [SwaggerOperation(Summary = "Logout account")]
    [SwaggerResponse(StatusCodes.Status200OK, "Account logged out successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.GetUserId();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("User {UserId} logged out", userId);
        return Ok();
    }

    [HttpPost("verify")]
    [SwaggerOperation(Summary = "Account email verification")]
    [SwaggerResponse(StatusCodes.Status200OK, "Account was verified")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid or expired verify code")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailDto dto)
    {
        var user = await _userService.GetUserFromVerifyCode(dto.VerifyCode);
        if (user is null)
            return BadRequest("Invalid or expired verify code");

        await _userService.VerifyUser(user);
        return Ok();
    }

    [HttpPost("resend-verify")]
    [Authorize(Policy = AuthorizationPolicies.ActiveUserOnly)]
    [SwaggerOperation(Summary = "Resend verify email link")]
    [SwaggerResponse(StatusCodes.Status200OK, "Resend verify email successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Account already verified")]
    public async Task<IActionResult> ResendVerifyEmail()
    {
        if (User.HasClaim(AdditionalClaimTypes.EmailVerified, bool.TrueString))
            return Forbid("Your account already verified");
        
        var emailClaim = User.FindFirst(c => c.Type == ClaimTypes.Email);
        await _userService.SendVerifyEmail(emailClaim!.Value, User.GetUserId());
        return Ok();
    }
}