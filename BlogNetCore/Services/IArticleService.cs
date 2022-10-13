using BlogNetCore.Models;

namespace BlogNetCore.Services;

public interface IArticleService
{
    Task<IEnumerable<Article>> GetFeedArticles();
    Task<Article?> GetArticleById(int id);
    Task<Article?> GetArticleBySlug(string slug);
    Task CreateArticle(string title, string content, int authorId, ICollection<Tag> tags);
    Task PublishArticle(Article article);
    Task ArchiveArticle(Article article);
    Task DeleteArticle(Article article);
}