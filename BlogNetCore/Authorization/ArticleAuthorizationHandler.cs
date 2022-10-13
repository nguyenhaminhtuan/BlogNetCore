using BlogNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace BlogNetCore.Authorization;

public class ArticleAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, Article>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Article resource)
    {
        if (context.User.Identity?.Name is null)
        {
            if (requirement.Name == Operations.Read.Name && resource.Status == ArticleStatus.Published)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }

        var userId = int.Parse(context.User.Identity.Name);
        var isAuthor = userId == resource.AuthorId;
        var isAdmin = context.User.IsInRole(UserRole.Admin.ToString());
        
        if (isAdmin)
            context.Succeed(requirement);
        
        if (requirement.Name == Operations.Create.Name)
            context.Succeed(requirement);
        
        if (requirement.Name == Operations.Read.Name &&
            (resource.Status == ArticleStatus.Published) || isAuthor)
            context.Succeed(requirement);
        
        if (requirement.Name == Operations.Update.Name && isAuthor)
            context.Succeed(requirement);
        
        if (requirement.Name == Operations.Delete.Name && isAuthor)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}