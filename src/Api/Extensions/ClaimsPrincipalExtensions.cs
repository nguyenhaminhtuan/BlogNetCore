using System.Security.Authentication;
using System.Security.Claims;

namespace Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var userIdClaim = claimsPrincipal.FindFirst(c => c.Type == ClaimTypes.Name);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
            throw new AuthenticationException();

        return userId;
    }
}