using System.Security.Authentication;
using System.Security.Claims;

namespace Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var userIdClaim = claimsPrincipal.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
            throw new AuthenticationException();

        return userId;
    }
}