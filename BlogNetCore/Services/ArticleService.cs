using BlogNetCore.Common.Utils;
using BlogNetCore.Data;
using BlogNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogNetCore.Services;

public class ArticleService : IArticleService
{
    private readonly ILogger<ArticleService> _logger;
    private readonly ApplicationDbContext _db;

    public ArticleService(
        ILogger<ArticleService> logger,
        ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task<IEnumerable<Article>> GetFeedArticles()
    {
        return await _db.Articles
            .Include(a => a.Author)
            .Include(a => a.Tags)
            .Where(a => a.Status == ArticleStatus.Published)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync();
    }

    public async Task<Article?> GetArticleById(int id)
    {
        return await _db.Articles
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Article?> GetArticleBySlug(string slug)
    {
        return await _db.Articles
            .Include(a => a.Author)
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => string.Equals(a.Slug, slug));
    }

    public async Task CreateArticle(string title, string content, int authorId, ICollection<Tag> tags)
    {
        var article = new Article()
        {
            Title = title,
            Slug = GenerateUtils.Slugify(title, (await GenerateUtils.GenerateShortId())),
            Content = content,
            Status = ArticleStatus.Draft,
            Tags = tags,
            AuthorId = authorId
        };
        await _db.AddAsync(article);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Article {ArticleId} was created in draft", article.Id);
    }

    private async Task ChangeArticleStatus(Article article, ArticleStatus status)
    {
        article.Status = status;
        if (status == ArticleStatus.Published)
            article.PublishedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        _logger.LogInformation(
            "Article {ArticleId} changed status to {ArticleStatus}",
            article.Id, Enum.GetName(typeof(ArticleStatus), article.Status)?.ToLower());
    }

    public Task PublishArticle(Article article)
    {
        return ChangeArticleStatus(article, ArticleStatus.Published);
    }

    public Task ArchiveArticle(Article article)
    {
        return ChangeArticleStatus(article, ArticleStatus.Archived);
    }

    public async Task DeleteArticle(Article article)
    {
        if (article.Status != ArticleStatus.Draft)
        {
            await ChangeArticleStatus(article, ArticleStatus.Deleted);
            return;
        };

        _db.Remove(article);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Draft article {ArticleId} was deleted", article.Id);
    }

    public async Task UpdateArticle(Article article, string title, string content, ICollection<Tag> tags)
    {
        article.Title = title;
        article.Slug = GenerateUtils.Slugify(title, (await GenerateUtils.GenerateShortId()));
        article.Content = content;
        article.Tags = tags;
        article.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}