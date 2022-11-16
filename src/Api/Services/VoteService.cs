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

    public async Task<Vote?> GetVoteById(int voteId)
    {
        return await _db.Votes.FirstOrDefaultAsync(v => v.Id == voteId);
    }

    public async Task<Vote?> GetArticleVoteByUser(int userId, Article article)
    {
        return await _db.Votes.FirstOrDefaultAsync(v => v.ArticleId == article.Id && v.OwnerId == userId);
    }

    public async Task<Vote?> GetCommentVoteByUser(int userId, Comment comment)
    {
        return await _db.Votes.FirstOrDefaultAsync(v => v.CommentId == comment.Id && v.OwnerId == userId);
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

    public async Task DeleteVote(Vote vote)
    {
        _db.Votes.Remove(vote);
        await _db.SaveChangesAsync();
    }
}