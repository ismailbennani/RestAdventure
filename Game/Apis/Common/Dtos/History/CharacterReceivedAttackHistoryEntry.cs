using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Characters;
using RestAdventure.Game.Apis.Common.Dtos.Combats;

namespace RestAdventure.Game.Apis.Common.Dtos.History;

/// <summary>
///     Character attacked history entry
/// </summary>
public class CharacterReceivedAttackHistoryEntryDto : CharacterHistoryEntryDto
{
    /// <summary>
    ///     The attack dealt by the character
    /// </summary>
    [Required]
    public required EntityAttackDto AttackDealt { get; init; }

    /// <summary>
    ///     The attack received by the target
    /// </summary>
    [Required]
    public required EntityAttackDto AttackReceived { get; init; }

    /// <summary>
    ///     The unique ID of the target receiving the attack
    /// </summary>
    [Required]
    public required Guid AttackerId { get; init; }

    /// <summary>
    ///     The name of the target receiving the attack
    /// </summary>
    [Required]
    public required string AttackerName { get; init; }
}

static class CharacterReceivedAttackHistoryEntryMappingExtensions
{
    public static CharacterReceivedAttackHistoryEntryDto ToDto(this CharacterReceivedAttackHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            AttackDealt = entry.AttackDealt.ToDto(),
            AttackReceived = entry.AttackReceived.ToDto(),
            AttackerId = entry.AttackerId.Guid,
            AttackerName = entry.AttackerName
        };
}
