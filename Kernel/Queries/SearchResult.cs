namespace RestAdventure.Kernel.Queries;

public class SearchResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
    public required int TotalItemsCount { get; init; }
    public required int TotalPagesCount { get; init; }
}

public static class SearchResultExtensions
{
    public static SearchResult<TResult> Select<TSource, TResult>(this SearchResult<TSource> result, Func<TSource, TResult> selector) =>
        new()
        {
            Items = result.Items.Select(selector).ToArray(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalItemsCount = result.TotalItemsCount,
            TotalPagesCount = result.TotalPagesCount
        };
}
