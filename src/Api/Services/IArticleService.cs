using Api.Models;

namespace Api.Services;

public interface IArticleService
{
    Task<IEnumerable<Article>> GetFeedArticlesAsync();
    Task<Article?> GetArticleByIdAsync(int id);
    Task<Article?> GetArticleBySlugAsync(string slug);
    Task<Article> CreateArticleAsync(string title, string content, User author, ISet<Tag> tags);
    Task PublishArticleAsync(Article article);
    Task ArchiveArticleAsync(Article article);
    Task DeleteArticleAsync(Article article);
    Task UpdateArticleAsync(Article article, string title, string content, ISet<Tag> tags);
}