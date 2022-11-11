using Api.Models;

namespace Api.Services;

public class CommentService : ICommentService
{
    public Task CreateArticleComment(string body, int ownerId, int articleId)
    {
        throw new NotImplementedException();
    }

    public Task ReplyComment(string body, int commentId, int ownerId, int replyToId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteComment(int commentId)
    {
        throw new NotImplementedException();
    }

    public Task UpvoteComment(int commentId, int voterId)
    {
        throw new NotImplementedException();
    }

    public Task DownvoteComment(int commentId, int voterId)
    {
        throw new NotImplementedException();
    }

    public Task<PaginatedList<Comment>> GetCommentsByArticlePagination(int pageIndex, int pageSize, int articleId)
    {
        throw new NotImplementedException();
    }

    public Task<PaginatedList<Comment>> GetRepliesByCommentPagination(int pageIndex, int pageSize, int commentId)
    {
        throw new NotImplementedException();
    }
}