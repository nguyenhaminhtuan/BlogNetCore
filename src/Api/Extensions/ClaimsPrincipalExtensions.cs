using System.Security.Authentication;
using System.Security.Claims;
using Api.Auth;

namespace Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var userIdClaim = claimsPrincipal.Claims
            .FirstOrDefault(c => c.Type == CookieClaimTypes.Identity);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
            throw new AuthenticationException();

        return userId;
    }
}