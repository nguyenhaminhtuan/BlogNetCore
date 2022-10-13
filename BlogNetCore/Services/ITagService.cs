using BlogNetCore.Models;

namespace BlogNetCore.Services;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetTagsById(ISet<int> tagIds);
}