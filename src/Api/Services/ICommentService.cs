using Api.Models;

namespace Api.Services;

public interface ICommentService
{
    public Task CreateArticleComment(string body, int ownerId, int articleId);
    public Task ReplyComment(string body, int commentId, int ownerId, int replyToId);
    public Task DeleteComment(int commentId);
    public Task UpvoteComment(int commentId, int voterId);
    public Task DownvoteComment(int commentId, int voterId);
    public Task<PaginatedList<Comment>> GetCommentsByArticlePagination(int pageIndex, int pageSize, int articleId);
    public Task<PaginatedList<Comment>> GetRepliesByCommentPagination(int pageIndex, int pageSize, int commentId);
}