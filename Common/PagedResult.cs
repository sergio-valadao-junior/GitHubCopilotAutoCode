namespace GitHubCopilotAutoCode.Common;

/// <summary>
/// A generic result container for paginated data.
/// </summary>
/// <typeparam name="T">The type of items in the page.</typeparam>
public sealed record PagedResult<T>(
    /// <summary>
    /// The items in the current page.
    /// </summary>
    List<T> Items,

    /// <summary>
    /// The current page number (1-based).
    /// </summary>
    int PageNumber,

    /// <summary>
    /// The number of items per page.
    /// </summary>
    int PageSize,

    /// <summary>
    /// The total number of items across all pages.
    /// </summary>
    int TotalCount,

    /// <summary>
    /// The total number of pages available.
    /// </summary>
    int TotalPages)
{
    /// <summary>
    /// Indicates whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Indicates whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}
