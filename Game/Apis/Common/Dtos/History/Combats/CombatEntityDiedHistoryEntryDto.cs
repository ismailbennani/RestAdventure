using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Combats;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Combats;

/// <summary>
///     Combat entity died history entry
/// </summary>
public class CombatEntityDiedHistoryEntryDto : CombatHistoryEntryDto
{
    /// <summary>
    ///     The unique ID of the entity that died
    /// </summary>
    [Required]
    public required Guid EntityId { get; init; }

    /// <summary>
    ///     The name of the entity that died
    /// </summary>
    [Required]
    public required string EntityName { get; init; }

    /// <summary>
    ///     The unique ID of the entity that attacked
    /// </summary>
    [Required]
    public required Guid AttackerId { get; init; }

    /// <summary>
    ///     The name of the entity that attacked
    /// </summary>
    [Required]
    public required string AttackerName { get; init; }
}

static class CombatEntityDiedHistoryEntryMappingExtensions
{
    public static CombatEntityDiedHistoryEntryDto ToDto(this CombatEntityDiedHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            Turn = entry.Turn,
            AttackerId = entry.AttackerId.Guid,
            AttackerName = entry.AttackerName,
            EntityId = entry.EntityId.Guid,
            EntityName = entry.EntityName
        };
}
