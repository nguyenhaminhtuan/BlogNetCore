using System.Text;
using System.Text.RegularExpressions;
using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

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

    private static string GenerateSlug(string s)
    {
        return Regex.Replace(s, "[^A-Za-z0-9 ]", "")
            .Normalize(NormalizationForm.FormD)
            .Replace(" ", "-");
    }

    public async Task<IEnumerable<Article>> GetFeedArticlesAsync()
    {
        return await _db.Articles
            .Include(a => a.Author)
            .Include(a => a.Tags)
            .Where(a => a.Status == ArticleStatus.Published)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync();
    }

    public Task<Article?> GetArticleByIdAsync(int id)
    {
        return _db.Articles
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public Task<Article?> GetArticleBySlugAsync(string slug)
    {
        return _db.Articles
            .Include(a => a.Author)
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => string.Equals(a.Slug, slug));
    }

    public async Task<Article> CreateArticleAsync(string title, string content, User author, ISet<Tag> tags)
    {
        var article = new Article()
        {
            Title = title,
            Slug = GenerateSlug(title),
            Content = content,
            Status = ArticleStatus.Draft,
            Tags = tags,
            Author = author
        };
        await _db.AddAsync(article);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Article {ArticleId} was created in draft", article.Id);
        return article;
    }

    private async Task ChangeArticleStatusAsync(Article article, ArticleStatus status)
    {
        article.Status = status;
        if (status == ArticleStatus.Published)
            article.PublishedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        _logger.LogInformation(
            "Article {ArticleId} changed status to {ArticleStatus}",
            article.Id, Enum.GetName(typeof(ArticleStatus), article.Status)?.ToLower());
    }

    public Task PublishArticleAsync(Article article)
    {
        return article.Status == ArticleStatus.Published
            ? Task.CompletedTask
            : ChangeArticleStatusAsync(article, ArticleStatus.Published);
    }

    public Task ArchiveArticleAsync(Article article)
    {
        return article.Status == ArticleStatus.Archived
            ? Task.CompletedTask
            : ChangeArticleStatusAsync(article, ArticleStatus.Archived);
    }

    public async Task DeleteArticleAsync(Article article)
    {
        if (article.Status == ArticleStatus.Draft)
        {
            _db.Remove(article);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Draft article {ArticleId} was deleted", article.Id);
        }
        
        if (article.Status != ArticleStatus.Deleted)
            await ChangeArticleStatusAsync(article, ArticleStatus.Deleted);
    }

    public async Task UpdateArticleAsync(Article article, string title, string content, ISet<Tag> tags)
    {
        article.Title = title;
        article.Slug = GenerateSlug(title);
        article.Content = content;
        article.Tags = tags;
        article.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        _logger.LogInformation("Article {ArticleId} updated", article.Id);
    }
}