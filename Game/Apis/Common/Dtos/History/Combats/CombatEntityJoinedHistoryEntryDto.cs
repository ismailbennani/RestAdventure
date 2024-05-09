using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat;
using RestAdventure.Core.History.Combats;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Combats;

/// <summary>
///     Combat entity joined history entry
/// </summary>
public class CombatEntityJoinedHistoryEntryDto : CombatHistoryEntryDto
{
    /// <summary>
    ///     The unique ID of the entity that joined the combat
    /// </summary>
    [Required]
    public required Guid EntityId { get; init; }

    /// <summary>
    ///     The name of the entity that joined the combat
    /// </summary>
    [Required]
    public required string EntityName { get; init; }

    /// <summary>
    ///     The side of the combat joined by the entity
    /// </summary>
    [Required]
    public required CombatSide Side { get; init; }
}

static class CombatEntityJoinedHistoryEntryMappingExtensions
{
    public static CombatEntityJoinedHistoryEntryDto ToDto(this CombatEntityJoinedHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            Turn = entry.Turn,
            EntityId = entry.EntityId.Guid,
            EntityName = entry.EntityName,
            Side = entry.Side
        };
}
