using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Characters;
using RestAdventure.Game.Apis.Common.Dtos.History.Common;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Characters;

/// <summary>
///     Character combat in preparation canceled
/// </summary>
public class CharacterCombatStartedHistoryEntryDto : CharacterHistoryEntryDto
{
    /// <summary>
    ///     The unique ID of the combat instance
    /// </summary>
    [Required]
    public required Guid CombatInstanceId { get; init; }

    /// <summary>
    ///     The unique ID of the location of the combat
    /// </summary>
    [Required]
    public required Guid LocationId { get; init; }

    /// <summary>
    ///     The unique ID of the area of the location of the combat
    /// </summary>
    [Required]
    public required Guid LocationAreaId { get; init; }

    /// <summary>
    ///     The name of the area of the location of the combat
    /// </summary>
    [Required]
    public required string LocationAreaName { get; init; }

    /// <summary>
    ///     The X position of the location of the combat
    /// </summary>
    [Required]
    public required int LocationPositionX { get; init; }

    /// <summary>
    ///     The Y position of the location of the combat
    /// </summary>
    [Required]
    public required int LocationPositionY { get; init; }

    /// <summary>
    ///     The attackers of the combat
    /// </summary>
    [Required]
    public required IReadOnlyList<CombatEntityInHistoryEntryDto> Attackers { get; init; }

    /// <summary>
    ///     The defenders of the combat
    /// </summary>
    [Required]
    public required IReadOnlyList<CombatEntityInHistoryEntryDto> Defenders { get; init; }
}

static class CharacterCombatStartedHistoryEntryMappingExtensions
{
    public static CharacterCombatStartedHistoryEntryDto ToDto(this CharacterCombatStartedHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            CombatInstanceId = entry.CombatInstanceId.Guid,
            LocationId = entry.LocationId.Guid,
            LocationAreaId = entry.LocationAreaId.Guid,
            LocationAreaName = entry.LocationAreaName,
            LocationPositionX = entry.LocationPositionX,
            LocationPositionY = entry.LocationPositionY,
            Attackers = entry.Attackers.Select(e => e.ToDto()).ToArray(),
            Defenders = entry.Defenders.Select(e => e.ToDto()).ToArray()
        };
}
