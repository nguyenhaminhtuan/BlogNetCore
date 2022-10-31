using System.Security.Claims;
using Api.Extensions;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Api.Authorization;

public class ArticleAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, Article>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Article resource)
    {
        if (requirement.Name == ArticleOperations.Create.Name && CanCreate(context.User))
            context.Succeed(requirement);

        if (requirement.Name == ArticleOperations.Read.Name && CanRead(context.User, resource))
            context.Succeed(requirement);

        if (requirement.Name == ArticleOperations.Update.Name && CanUpdate(context.User, resource))
            context.Succeed(requirement);

        if (requirement.Name == ArticleOperations.Delete.Name && CanDelete(context.User, resource))
            context.Succeed(requirement);
        
        if (requirement.Name == ArticleOperations.Publish.Name && CanPublish(context.User, resource))
            context.Succeed(requirement);
        
        if (requirement.Name == ArticleOperations.Archive.Name && CanArchive(context.User, resource))
            context.Succeed(requirement);
        
        return Task.CompletedTask;
    }

    private static bool CanCreate(ClaimsPrincipal user)
    {
        return IsAuthenticated(user);
    }

    private static bool CanRead(ClaimsPrincipal user, Article article)
    {
        if (IsAdmin(user) && article.Status != ArticleStatus.Draft)
            return true;
        
        if (IsAuthor(user, article))
            return article.Status != ArticleStatus.Deleted;

        return IsPublished(article);
    }
    
    private static bool CanUpdate(ClaimsPrincipal user, Article article)
    {
        return IsAuthor(user, article) && article.Status <= ArticleStatus.Published;
    }

    private static bool CanDelete(ClaimsPrincipal user, Article article)
    {
        return (IsAuthor(user, article) && article.Status <= ArticleStatus.Published) ||
               (IsAdmin(user) && article.Status is not ArticleStatus.Draft or ArticleStatus.Deleted);
    }

    private static bool CanPublish(ClaimsPrincipal user, Article article)
    {
        return IsAuthor(user, article) & article.Status == ArticleStatus.Draft;
    }

    private static bool CanArchive(ClaimsPrincipal user, Article article)
    {
        return IsAuthor(user, article) & article.Status == ArticleStatus.Published;
    }

    private static bool IsAuthenticated(ClaimsPrincipal user)
    {
        return user.Identity is not null;
    }

    private static bool IsAuthor(ClaimsPrincipal user, Article resource)
    {
        return IsAuthenticated(user) && user.GetUserId() == resource.AuthorId;
    }

    private static bool IsAdmin(ClaimsPrincipal user)
    {
        return IsAuthenticated(user) && user.IsInRole(UserRole.Admin.ToString());
    }

    private static bool IsPublished(Article article)
    {
        return article.Status == ArticleStatus.Published;
    }
}