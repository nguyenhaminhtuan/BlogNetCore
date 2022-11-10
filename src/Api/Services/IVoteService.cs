using Api.Models;

namespace Api.Services;

public interface IVoteService
{
    public Task UpvoteArticle(int articleId, int ownerId);
    public Task DownvoteArticle(int articleId, int ownerId);
    public Task UpvoteComment(int commentId, int ownerId);
    public Task DownvoteComment(int commentId, int ownerId);
    public Task<IEnumerable<Vote>> GetVotesByArticle(int articleId);
    public Task<IEnumerable<Vote>> GetVotesByComment(int commentId);
    public int CountArticleUpvote(IEnumerable<Vote> votes);
    public int CountArticleDownvote(IEnumerable<Vote> votes);
}