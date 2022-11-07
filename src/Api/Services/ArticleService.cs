using System.Text;
using System.Text.RegularExpressions;
using Api.Data;
using Api.Exceptions;
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

    public async Task<PaginatedList<Article>> GetPublishedArticlesPagination(int pageIndex, int pageSize)
    {
        var source = _db.Articles
            .AsNoTracking()
            .Include(a => a.Author)
            .Include(a => a.Tags)
            .Where(a => a.Status == ArticleStatus.Published)
            .OrderByDescending(a => a.PublishedAt);
        return await PaginatedList<Article>.CreateAsync(source, pageIndex, pageSize);
    }

    public Task<Article?> GetArticleById(int id)
    {
        return _db.Articles
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public Task<Article?> GetArticleBySlug(string slug)
    {
        return _db.Articles
            .Include(a => a.Author)
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => string.Equals(a.Slug, slug));
    }

    public async Task<Article> CreateArticle(string title, string content, User author, ISet<Tag> tags)
    {
        var slug = GenerateSlug(title);
        if (await _db.Articles.AnyAsync(a => string.Equals(a.Title, slug)))
            throw new ConflictException("Title given was taken");
            
        var article = new Article()
        {
            Title = title,
            Slug = slug,
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
        return article.Status == ArticleStatus.Published
            ? Task.CompletedTask
            : ChangeArticleStatus(article, ArticleStatus.Published);
    }

    public Task ArchiveArticle(Article article)
    {
        return article.Status == ArticleStatus.Archived
            ? Task.CompletedTask
            : ChangeArticleStatus(article, ArticleStatus.Archived);
    }

    public async Task DeleteArticle(Article article)
    {
        if (article.Status == ArticleStatus.Draft)
        {
            _db.Remove(article);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Draft article {ArticleId} was deleted", article.Id);
        }
        
        if (article.Status != ArticleStatus.Deleted)
            await ChangeArticleStatus(article, ArticleStatus.Deleted);
    }

    public async Task UpdateArticle(Article article, string title, string content, ISet<Tag> tags)
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