using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class TagService : ITagService
{
    private readonly ApplicationDbContext _db;

    public TagService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Tag>> GetTagsById(ISet<int> tagIds)
    {
        return await _db.Tags
            .Where(t => tagIds.Contains(t.Id))
            .ToListAsync();
    }
}