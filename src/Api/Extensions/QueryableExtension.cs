using Api.Models;

namespace Api.Extensions;

public static class QueryableExtension
{
    public static IQueryable<TSource> Sort<TSource, TKey>(this IQueryable<TSource> source,
        Func<TSource, TKey> keySelector, SortDirection direction)
    {
        return (direction == SortDirection.ASC
            ? source.OrderBy(keySelector)
            : source.OrderByDescending(keySelector)).AsQueryable();
    }
}