using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BlogNetCore.Config;

public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
{
    public override async Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        await Results.Problem(
                title: "Unauthorized request.",
                statusCode: (int)HttpStatusCode.Unauthorized)
            .ExecuteAsync(context.HttpContext);
    }

    public override async Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
    {
        await Results.Problem(
                title: "Unauthorized request.",
                statusCode: (int)HttpStatusCode.Unauthorized)
            .ExecuteAsync(context.HttpContext);
    }
}