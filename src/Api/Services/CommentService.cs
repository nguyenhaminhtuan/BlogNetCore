using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _db;

    public CommentService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Comment?> GetCommentById(int id)
    {
        return await _db.Comments.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task CreateArticleComment(string body, int ownerId, Article article)
    {
        var comment = new Comment()
        {
            Body = body,
            OwnerId = ownerId,
            ArticleId = article.Id
        };
        await _db.Comments.AddAsync(comment);
        await _db.SaveChangesAsync();
    }

    public async Task ReplyComment(string body, int ownerId, Comment comment, int? replyToId)
    {
        var reply = new Comment()
        {
            Body = body,
            OwnerId = ownerId,
            ArticleId = comment.ArticleId,
            ReplyFromId = comment.Id,
            ReplyToId = replyToId
        };
        await _db.Comments.AddAsync(reply);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteComment(Comment comment)
    {
        comment.IsDeleted = true;
        await _db.SaveChangesAsync();
    }

    public async Task<PaginatedList<Comment>> GetCommentsByArticlePagination(int pageIndex, int pageSize, int articleId)
    {
        var queryable = _db.Comments
            .AsNoTracking()
            .Include(c => c.Owner)
            .Include(c => c.Replies.Take(5).OrderBy(c => c.CommentedAt))
            .Include(c => c.Replies).ThenInclude(r => r.Owner)
            .Include(c => c.Replies).ThenInclude(r => r.ReplyTo)
            .Where(c => c.ArticleId == articleId)
            .Where(c => c.ReplyFromId == null)
            .OrderBy(c => c.CommentedAt)
            .Select(c => new Comment()
            {
                Id = c.Id,
                Body = c.Body,
                IsDeleted = c.IsDeleted,
                CommentedAt = c.CommentedAt,
                Owner = c.Owner,
                Replies = c.Replies,
                UpVoteCount = c.Votes.Count(v => v.IsPositive),
                DownVoteCount = c.Votes.Count(v => !v.IsPositive)
            });
        return await PaginatedList<Comment>.CreateAsync(queryable, pageIndex, pageSize);
    }

    public async Task<PaginatedList<Comment>> GetRepliesByCommentPagination(int pageIndex, int pageSize, int commentId)
    {
        var queryable = _db.Comments
            .AsNoTracking()
            .Include(c => c.Owner)
            .Include(c => c.ReplyTo)
            .Where(c => c.ReplyFromId == commentId)
            .OrderBy(c => c.CommentedAt)
            .Select(c => new Comment()
            {
                Id = c.Id,
                Body = c.Body,
                IsDeleted = c.IsDeleted,
                CommentedAt = c.CommentedAt,
                Owner = c.Owner,
                ReplyTo = c.ReplyTo,
                UpVoteCount = c.Votes.Count(v => v.IsPositive),
                DownVoteCount = c.Votes.Count(v => !v.IsPositive),
            });
        return await PaginatedList<Comment>.CreateAsync(queryable, pageIndex, pageSize);
    }
}