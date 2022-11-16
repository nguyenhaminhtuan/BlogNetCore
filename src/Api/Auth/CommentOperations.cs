using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Api.Auth;

public static class CommentOperations
{
    public static readonly OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
    public static readonly OperationAuthorizationRequirement Vote = new() { Name = nameof(Vote) };
    public static readonly OperationAuthorizationRequirement Reply = new() { Name = nameof(Reply) };
}