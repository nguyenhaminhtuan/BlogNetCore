using Api.Extensions;
using Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Api.Auth;

public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
{
    private readonly IUserService _userService;

    public CustomCookieAuthenticationEvents(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var userPrincipal = context.Principal!;
        var lastChangedClaim = userPrincipal.FindFirst(c => c.Type == AdditionalClaimTypes.LastChanged);
        if (string.IsNullOrWhiteSpace(lastChangedClaim?.Value))
        {
            await Reject(context);
            return;
        }

        if (!DateTime.TryParse(lastChangedClaim.Value, out var lastChanged))
        {
            await Reject(context);
            return;
        }
        
        var user = await _userService.GetUserById(userPrincipal.GetUserId());
        if (user is null || DateTime.Compare(user.LastChanged, lastChanged) != 0)
            await Reject(context);
    }

    private static async Task Reject(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
}