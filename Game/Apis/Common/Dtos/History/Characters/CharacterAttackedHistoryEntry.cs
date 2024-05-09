using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Characters;
using RestAdventure.Game.Apis.Common.Dtos.Combats;

namespace RestAdventure.Game.Apis.Common.Dtos.History;

/// <summary>
///     Character attacked history entry
/// </summary>
public class CharacterAttackedHistoryEntryDto : CharacterHistoryEntryDto
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
    public required Guid TargetId { get; init; }

    /// <summary>
    ///     The name of the target receiving the attack
    /// </summary>
    [Required]
    public required string TargetName { get; init; }
}

static class CharacterAttackedHistoryEntryMappingExtensions
{
    public static CharacterAttackedHistoryEntryDto ToDto(this CharacterAttackedHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            AttackDealt = entry.AttackDealt.ToDto(),
            AttackReceived = entry.AttackReceived.ToDto(),
            TargetId = entry.TargetId.Guid,
            TargetName = entry.TargetName
        };
}
