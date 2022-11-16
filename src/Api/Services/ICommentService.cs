using Api.Models;

namespace Api.Services;

public interface ICommentService
{
    public Task<Comment?> GetCommentById(int id);
    public Task CreateArticleComment(string body, int ownerId, Article article);
    public Task ReplyComment(string body, int ownerId, Comment comment, int? replyToId);
    public Task DeleteComment(Comment comment);
    public Task<PaginatedList<Comment>> GetCommentsByArticlePagination(int pageIndex, int pageSize, int articleId);
    public Task<PaginatedList<Comment>> GetRepliesByCommentPagination(int pageIndex, int pageSize, int commentId);
}