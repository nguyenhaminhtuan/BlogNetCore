using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class VoteService : IVoteService
{
    private readonly ApplicationDbContext _db;

    public VoteService(ApplicationDbContext db)
    {
        _db = db;
    }

    private async Task CreateVote(bool isPositive, int ownerId, int? articleId, int? commentId)
    {
        if (articleId is null && commentId is null)
            throw new ArgumentNullException($"{nameof(articleId)}, {nameof(commentId)}");
        
        var vote = new Vote()
        {
            IsPositive = isPositive,
            ArticleId = articleId,
            CommentId = commentId,
            OwnerId = ownerId,
        };
        await _db.Votes.AddAsync(vote);
        await _db.SaveChangesAsync();
    }

    public async Task UpvoteArticle(int articleId, int ownerId)
    {
        await CreateVote(true, ownerId, articleId, null);
    }

    public async Task DownvoteArticle(int articleId, int ownerId)
    {
        await CreateVote(false, ownerId, articleId, null);
    }

    public async Task UpvoteComment(int commentId, int ownerId)
    {
        await CreateVote(true, ownerId, null, commentId);
    }

    public async Task DownvoteComment(int commentId, int ownerId)
    {
        await CreateVote(false, ownerId, null, commentId);
    }

    public async Task<IEnumerable<Vote>> GetVotesByArticle(int articleId)
    {
        return await _db.Votes
            .Include(v => v.Article)
            .Where(v => v.ArticleId != null)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vote>> GetVotesByComment(int commentId)
    {
        return await _db.Votes
            .Include(v => v.Comment)
            .Where(v => v.Comment != null)
            .ToListAsync();
    }

    public int CountArticleUpvote(IEnumerable<Vote> votes)
    {
        return votes.Count(v => v.IsPositive);
    }

    public int CountArticleDownvote(IEnumerable<Vote> votes)
    {
        return votes.Count(v => !v.IsPositive);
    }
}