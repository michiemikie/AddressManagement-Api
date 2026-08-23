namespace AddressManagement.Application.DTOs;

/// <summary>
/// Generic wrapper for paginated list results, so clients know not just
/// the current page of items but also how many pages/items exist in total.
/// </summary>
public class PagedResultDto<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}