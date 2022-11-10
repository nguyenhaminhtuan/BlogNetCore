using Api.Models;

namespace Api.Services;

public interface IArticleService
{
    public Task<PaginatedList<Article>> GetPublishedArticlesByPagination(int pageIndex, int pageSize);
    public Task<Article?> GetArticleById(int id);
    public Task<Article?> GetArticleBySlug(string slug);
    public Task<Article> CreateArticle(string title, string content, int authorId, ISet<Tag> tags);
    public Task PublishArticle(Article article);
    public Task ArchiveArticle(Article article);
    public Task DeleteArticle(Article article);
    public Task UpdateArticle(Article article, string title, string content, ISet<Tag> tags);
    public Task<int> CountPublishedArticlesByAuthor(int authorId);
    public Task<PaginatedList<Article>> GetArticlesByAuthorFilterPagination(int authorId, int pageIndex, int pageSize,
        ArticleStatus status);
    public Task<PaginatedList<Article>> GetPublishedArticlesByTagPagination(int tagId, int pageIndex, int pageSize);
}