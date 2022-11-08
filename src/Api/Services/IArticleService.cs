using Api.Models;

namespace Api.Services;

public interface IArticleService
{
    Task<PaginatedList<Article>> GetPublishedArticlesByPagination(PaginateParams paginate);
    Task<Article?> GetArticleById(int id);
    Task<Article?> GetArticleBySlug(string slug);
    Task<Article> CreateArticle(string title, string content, User author, ISet<Tag> tags);
    Task PublishArticle(Article article);
    Task ArchiveArticle(Article article);
    Task DeleteArticle(Article article);
    Task UpdateArticle(Article article, string title, string content, ISet<Tag> tags);
    Task<int> CountPublishedArticlesByAuthor(int authorId);
    Task<PaginatedList<Article>> GetPublishedArticlesByAuthorPagination(int authorId, PaginateParams paginate);
}