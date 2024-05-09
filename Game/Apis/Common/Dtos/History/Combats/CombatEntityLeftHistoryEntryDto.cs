using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat;
using RestAdventure.Core.History.Combats;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Combats;

/// <summary>
///     Combat entity left history entry
/// </summary>
public class CombatEntityLeftHistoryEntryDto : CombatHistoryEntryDto
{
    /// <summary>
    ///     The unique ID of the entity that left the combat
    /// </summary>
    [Required]
    public required Guid EntityId { get; init; }

    /// <summary>
    ///     The name of the entity that left the combat
    /// </summary>
    [Required]
    public required string EntityName { get; init; }

    /// <summary>
    ///     The side of the combat left by the entity
    /// </summary>
    [Required]
    public required CombatSide Side { get; init; }
}

static class CombatEntityLeftHistoryEntryMappingExtensions
{
    public static CombatEntityLeftHistoryEntryDto ToDto(this CombatEntityLeftHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            Turn = entry.Turn,
            EntityId = entry.EntityId.Guid,
            EntityName = entry.EntityName,
            Side = entry.Side
        };
}
