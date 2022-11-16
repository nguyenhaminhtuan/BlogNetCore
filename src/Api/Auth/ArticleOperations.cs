using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Api.Auth;

public static class ArticleOperations
{
    public static readonly OperationAuthorizationRequirement Create = new() { Name = nameof(Create) };
    public static readonly OperationAuthorizationRequirement Read = new() { Name = nameof(Read) };
    public static readonly OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };
    public static readonly OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
    public static readonly OperationAuthorizationRequirement Publish = new() { Name = nameof(Publish) };
    public static readonly OperationAuthorizationRequirement Archive = new() { Name = nameof(Archive) };
    public static readonly OperationAuthorizationRequirement Vote = new() { Name = nameof(Vote) };
    public static readonly OperationAuthorizationRequirement Comment = new() { Name = nameof(Comment) };
}