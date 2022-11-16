using Api.Models;

namespace Api.Services;

public interface IVoteService
{
    public Task<Vote?> GetArticleVoteByUser(int userId, Article article);
    public Task<Vote?> GetCommentVoteByUser(int userId, Comment comment);
    public Task UpvoteArticle(int articleId, int ownerId);
    public Task DownvoteArticle(int articleId, int ownerId);
    public Task UpvoteComment(int commentId, int ownerId);
    public Task DownvoteComment(int commentId, int ownerId);
    public Task DeleteVote(Vote vote);
}