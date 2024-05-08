using System.ComponentModel.DataAnnotations;

namespace RestAdventure.Game.Apis.Common.Dtos.Maps;

/// <summary>
///     Location with access
/// </summary>
public class LocationWithAccessDto
{
    /// <summary>
    ///     The location
    /// </summary>
    [Required]
    public required LocationMinimalDto Location { get; init; }

    /// <summary>
    ///     Is the location accessible
    /// </summary>
    [Required]
    public required bool IsAccessible { get; init; }

    /// <summary>
    ///     Why is the location not accessible
    /// </summary>
    public string? WhyIsNotAccessible { get; init; }
}
