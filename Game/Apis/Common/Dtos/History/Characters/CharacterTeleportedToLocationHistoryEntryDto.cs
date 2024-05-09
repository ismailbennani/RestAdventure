using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Characters;

/// <summary>
///     Character teleported to location history entry
/// </summary>
public class CharacterTeleportedToLocationHistoryEntryDto : CharacterHistoryEntryDto
{
    /// <summary>
    ///     The unique ID of the old location
    /// </summary>
    [Required]
    public required Guid? OldLocationId { get; init; }

    /// <summary>
    ///     The X position of the old location
    /// </summary>
    [Required]
    public required int? OldLocationPositionX { get; init; }

    /// <summary>
    ///     The Y position of the old location
    /// </summary>
    [Required]
    public required int? OldLocationPositionY { get; init; }

    /// <summary>
    ///     The unique ID of the area of the old location
    /// </summary>
    [Required]
    public required Guid? OldLocationAreaId { get; init; }

    /// <summary>
    ///     The name of the area of the old location
    /// </summary>
    [Required]
    public required string? OldLocationAreaName { get; init; }

    /// <summary>
    ///     The unique ID of the new location
    /// </summary>
    [Required]
    public required Guid NewLocationId { get; init; }

    /// <summary>
    ///     The X position of the new location
    /// </summary>
    [Required]
    public required int NewLocationPositionX { get; init; }

    /// <summary>
    ///     The Y position of the new location
    /// </summary>
    [Required]
    public required int NewLocationPositionY { get; init; }

    /// <summary>
    ///     The unique ID of the area of the new location
    /// </summary>
    [Required]
    public required Guid NewLocationAreaId { get; init; }

    /// <summary>
    ///     The name of the area of the new location
    /// </summary>
    [Required]
    public required string NewLocationAreaName { get; init; }
}

static class CharacterTeleportedToLocationHistoryEntryMappingExtensions
{
    public static CharacterTeleportedToLocationHistoryEntryDto ToDto(this EntityTeleportedToLocationHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            OldLocationId = entry.OldLocationId?.Guid,
            OldLocationPositionX = entry.OldLocationPositionX,
            OldLocationPositionY = entry.OldLocationPositionY,
            OldLocationAreaId = entry.OldLocationAreaId?.Guid,
            OldLocationAreaName = entry.OldLocationAreaName,
            NewLocationId = entry.NewLocationId.Guid,
            NewLocationPositionX = entry.NewLocationPositionX,
            NewLocationPositionY = entry.NewLocationPositionY,
            NewLocationAreaId = entry.NewLocationAreaId.Guid,
            NewLocationAreaName = entry.NewLocationAreaName
        };
}
