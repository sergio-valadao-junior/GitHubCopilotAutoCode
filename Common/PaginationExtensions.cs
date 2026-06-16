using Microsoft.EntityFrameworkCore;

namespace GitHubCopilotAutoCode.Common;

/// <summary>
/// Extension methods for pagination operations.
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Applies pagination to an IQueryable sequence.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="query">The query to paginate.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>The paginated query.</returns>
    public static IQueryable<T> Paginate<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        var skipCount = (pageNumber - 1) * pageSize;
        return query.Skip(skipCount).Take(pageSize);
    }

    /// <summary>
    /// Applies pagination and returns a PagedResult with total count.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="query">The query to paginate.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the PagedResult.</returns>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        var totalCount = query.Count();
        var items = await query.Paginate(pageNumber, pageSize).ToListAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<T>(
            items,
            pageNumber,
            pageSize,
            totalCount,
            totalPages);
    }
}
