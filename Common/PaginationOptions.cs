namespace GitHubCopilotAutoCode.Common;

/// <summary>
/// Options for pagination and filtering queries.
/// </summary>
public sealed class PaginationOptions
{
    /// <summary>
    /// Default page size when not specified.
    /// </summary>
    private const int DefaultPageSize = 10;

    /// <summary>
    /// Maximum allowed page size.
    /// </summary>
    private const int MaxPageSize = 100;

    /// <summary>
    /// Initializes a new instance of the PaginationOptions class.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based). Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10, max 100.</param>
    /// <param name="searchTerm">Optional search term for filtering.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDirection">Sort direction: 'asc' or 'desc'. Defaults to 'asc'.</param>
    public PaginationOptions(
        int pageNumber = 1,
        int pageSize = DefaultPageSize,
        string? searchTerm = null,
        string? sortBy = null,
        string sortDirection = "asc")
    {
        PageNumber = Math.Max(1, pageNumber);
        PageSize = Math.Min(Math.Max(1, pageSize), MaxPageSize);
        SearchTerm = searchTerm?.Trim();
        SortBy = sortBy?.Trim();
        SortDirection = sortDirection?.ToLower() == "desc" ? "desc" : "asc";
    }

    /// <summary>
    /// Gets the page number (1-based).
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the page size (number of items per page).
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the search term for filtering, if provided.
    /// </summary>
    public string? SearchTerm { get; }

    /// <summary>
    /// Gets the field to sort by, if provided.
    /// </summary>
    public string? SortBy { get; }

    /// <summary>
    /// Gets the sort direction ('asc' or 'desc').
    /// </summary>
    public string SortDirection { get; }

    /// <summary>
    /// Gets the number of items to skip based on page number and size.
    /// </summary>
    public int SkipCount => (PageNumber - 1) * PageSize;
}
