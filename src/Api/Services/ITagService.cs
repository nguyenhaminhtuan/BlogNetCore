using Api.Models;

namespace Api.Services;

public interface ITagService
{
    public Task<IEnumerable<Tag>> GetTagsById(ISet<int> tagIds);
}