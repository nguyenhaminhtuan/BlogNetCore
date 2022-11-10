using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Api.Auth;

public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public CustomAuthorizationMiddlewareResultHandler(ProblemDetailsFactory problemDetailsFactory)
    {
        _problemDetailsFactory = problemDetailsFactory;
    }

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden && authorizeResult.AuthorizationFailure is not null)
        {
            var requirements = authorizeResult.AuthorizationFailure.FailedRequirements.ToList();
            var problem = _problemDetailsFactory.CreateProblemDetails(
                context,
                StatusCodes.Status403Forbidden,
                detail: "Permission denied");
            if (IsContainClaimsRequirement(requirements, AdditionalClaimTypes.IsDisabled))
            {
                problem.Detail = "Your account has been disabled";
                await context.Response.WriteAsJsonAsync(problem);
                return;
            }
            
            if (IsContainClaimsRequirement(requirements, AdditionalClaimTypes.EmailVerified))
            {
                problem.Detail = "Your account needs email verification";
                await context.Response.WriteAsJsonAsync(problem);
                return;
            }
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }

    private static bool IsContainClaimsRequirement(
        IEnumerable<IAuthorizationRequirement> requirements,
        string claimType)
    {
        return (requirements.OfType<ClaimsAuthorizationRequirement>()
            .FirstOrDefault(r => r.ClaimType == claimType)) is not null;
    }
}