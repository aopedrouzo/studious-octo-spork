namespace FootballManager.Application.Common;

public class PaginationParams
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; }
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public bool HasNextPage { get; }
    public bool HasPreviousPage { get; }

    public PaginatedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasNextPage = PageNumber < TotalPages;
        HasPreviousPage = PageNumber > 1;
    }
} 