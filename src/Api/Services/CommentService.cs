using Api.Models;

namespace Api.Services;

public class CommentService : ICommentService
{
    Task ICommentService.CreateArticleComment(string body, int ownerId, int articleId)
    {
        throw new NotImplementedException();
    }

    Task ICommentService.DeleteComment(int commentId)
    {
        throw new NotImplementedException();
    }

    Task ICommentService.DownvoteComment(int commentId, int voterId)
    {
        throw new NotImplementedException();
    }

    Task<PaginatedList<Comment>> ICommentService.GetCommentsByArticlePagination(int pageIndex, int pageSize, int articleId)
    {
        throw new NotImplementedException();
    }

    Task<PaginatedList<Comment>> ICommentService.GetRepliesByCommentPagination(int pageIndex, int pageSize, int commentId)
    {
        throw new NotImplementedException();
    }

    Task ICommentService.ReplyComment(string body, int commentId, int ownerId, int replyToId)
    {
        throw new NotImplementedException();
    }

    Task ICommentService.UpvoteComment(int commentId, int voterId)
    {
        throw new NotImplementedException();
    }
}