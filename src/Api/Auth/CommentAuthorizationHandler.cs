using System.Security.Claims;
using Api.Extensions;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Api.Auth;

public class CommentAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Comment>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Comment resource)
    {
        if (requirement.Name == CommentOperations.Delete.Name && CanDelete(context.User, resource))
            context.Succeed(requirement);
        
        if (requirement.Name == CommentOperations.Vote.Name && CanVote(resource))
            context.Succeed(requirement);
        
        if (requirement.Name == CommentOperations.Reply.Name && CanReply(resource))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }

    private static bool IsDeleted(Comment comment)
    {
        return comment.IsDeleted;
    }

    private static bool IsOwner(ClaimsPrincipal user, Comment comment)
    {
        return user.GetUserId() == comment.OwnerId;
    }

    private static bool IsParentComment(Comment comment)
    {
        return comment.ReplyFromId is null;
    }

    private static bool CanDelete(ClaimsPrincipal user, Comment comment)
    {
        return !IsDeleted(comment) && IsOwner(user, comment);
    }

    private static bool CanVote(Comment comment)
    {
        return !IsDeleted(comment);
    }

    private static bool CanReply(Comment comment)
    {
        return !IsDeleted(comment) && IsParentComment(comment);
    }
}