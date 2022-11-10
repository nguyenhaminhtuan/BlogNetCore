using Api.Models;

namespace Api.Services;

public class VoteService : IVoteService
{
    int IVoteService.CountArticleDownvote(IEnumerable<Vote> votes)
    {
        throw new NotImplementedException();
    }

    int IVoteService.CountArticleUpvote(IEnumerable<Vote> votes)
    {
        throw new NotImplementedException();
    }

    Task IVoteService.DownvoteArticle(int articleId, int ownerId)
    {
        throw new NotImplementedException();
    }

    Task IVoteService.DownvoteComment(int commentId, int ownerId)
    {
        throw new NotImplementedException();
    }

    Task<IEnumerable<Vote>> IVoteService.GetVotesByArticle(int articleId)
    {
        throw new NotImplementedException();
    }

    Task<IEnumerable<Vote>> IVoteService.GetVotesByComment(int commentId)
    {
        throw new NotImplementedException();
    }

    Task IVoteService.UpvoteArticle(int articleId, int ownerId)
    {
        throw new NotImplementedException();
    }

    Task IVoteService.UpvoteComment(int commentId, int ownerId)
    {
        throw new NotImplementedException();
    }
}