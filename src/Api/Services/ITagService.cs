using Api.Models;

namespace Api.Services;

public interface ITagService
{
    public Task<IEnumerable<Tag>> GetTagsById(ISet<int> tagIds);
    public Task<IEnumerable<Tag>> GetTags();
    public Task<Tag?> GetTagBySlug(string slug);
}