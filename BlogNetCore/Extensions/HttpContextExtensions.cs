namespace BlogNetCore.Extensions;

public static class HttpContextExtensions
{
    public static int GetCurrentUserId(this HttpContext httpContext)
    {
        if (httpContext.User.Identity?.Name is null)
            throw new ArgumentNullException(nameof(httpContext.User.Identity.Name));

        return int.Parse(httpContext.User.Identity.Name);
    }
}