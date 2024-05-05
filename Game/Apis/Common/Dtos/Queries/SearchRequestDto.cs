using System.ComponentModel.DataAnnotations;
using RestAdventure.Kernel.Queries;

namespace RestAdventure.Game.Apis.Common.Dtos.Queries;

/// <summary>
///     Common fields of a search request
/// </summary>
public class SearchRequestDto
{
    /// <summary>
    ///     The page number
    /// </summary>
    [Required]
    public required int PageNumber { get; init; }

    /// <summary>
    ///     The page size
    /// </summary>
    [Required]
    public required int PageSize { get; init; }
}

static class SearchRequestMappingExtensions
{
    public static PaginationParameters ToPaginationParameters(this SearchRequestDto request) => new() { PageNumber = request.PageNumber, PageSize = request.PageSize };
}
