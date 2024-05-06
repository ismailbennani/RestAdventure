using System.ComponentModel.DataAnnotations;
using RestAdventure.Kernel.Queries;

namespace RestAdventure.Game.Apis.Common.Dtos.Queries;

/// <summary>
///     Search result
/// </summary>
public class SearchResultDto<T>
{
    /// <summary>
    ///     The items found by the query
    /// </summary>
    [Required]
    public required IReadOnlyList<T> Items { get; init; }

    /// <summary>
    ///     The page number corresponding to the results that have been selected
    /// </summary>
    [Required]
    public required int PageNumber { get; init; }

    /// <summary>
    ///     The page size used by the search
    /// </summary>
    [Required]
    public required int PageSize { get; init; }

    /// <summary>
    ///     The total number of items matching the query
    /// </summary>
    [Required]
    public required int TotalItemsCount { get; init; }

    /// <summary>
    ///     The total number of pages
    /// </summary>
    [Required]
    public required int TotalPagesCount { get; init; }
}

static class SearchResultMappignExtensions
{
    public static SearchResultDto<T> ToDto<T>(this SearchResult<T> result) =>
        new()
        {
            Items = result.Items,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalItemsCount = result.TotalItemsCount,
            TotalPagesCount = result.TotalPagesCount
        };

    public static SearchResultDto<TDto> ToDto<T, TDto>(this SearchResult<T> result, Func<T, TDto> selector) =>
        new()
        {
            Items = result.Items.Select(selector).ToArray(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalItemsCount = result.TotalItemsCount,
            TotalPagesCount = result.TotalPagesCount
        };
}
