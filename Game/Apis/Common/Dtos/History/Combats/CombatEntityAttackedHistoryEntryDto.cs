using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Combats;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Combats;

/// <summary>
///     Combat entity attacked history entry
/// </summary>
public class CombatEntityAttackedHistoryEntryDto : CombatHistoryEntryDto
{
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

    /// <summary>
    ///     The unique ID of the entity that was attacked
    /// </summary>
    [Required]
    public required Guid TargetId { get; init; }

    /// <summary>
    ///     The name of the entity that was attacked
    /// </summary>
    [Required]
    public required string TargetName { get; init; }

    /// <summary>
    ///     The damages dealt by the attacker to the target
    /// </summary>
    [Required]
    public required int Damage { get; init; }
}

static class CombatEntityAttackedHistoryEntryMappingExtensions
{
    public static CombatEntityAttackedHistoryEntryDto ToDto(this CombatEntityAttackedHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            Turn = entry.Turn,
            AttackerId = entry.AttackerId.Guid,
            AttackerName = entry.AttackerName,
            TargetId = entry.TargetId.Guid,
            TargetName = entry.TargetName,
            Damage = entry.Damage
        };
}
