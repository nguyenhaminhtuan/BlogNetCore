using Api.Models;

namespace Api.Services;

public interface IVoteService
{
    public Task<Vote?> GetArticleVoteByUser(int userId, Article article);
    public Task<Vote?> GetCommentVoteByUser(int userId, Comment comment);
    public Task VoteArticle(int articleId, int ownerId, bool isPositive);
    public Task VoteComment(int commentId, int ownerId, bool isPositive);
    public Task DeleteVote(Vote vote);
}