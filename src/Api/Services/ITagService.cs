using Api.Models;

namespace Api.Services;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetTagsById(ISet<int> tagIds);
}