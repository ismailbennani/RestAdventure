using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat.Old;
using RestAdventure.Core.History.Combats;
using RestAdventure.Game.Apis.Common.Dtos.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.Combats;

/// <summary>
///     Archived combat
/// </summary>
public class ArchivedCombatDto
{
    /// <summary>
    ///     The unique ID of the combat
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The attackers in the combat
    /// </summary>
    [Required]
    public required IReadOnlyList<EntityMinimalDto> Attackers { get; init; }

    /// <summary>
    ///     The defenders in the combat
    /// </summary>
    [Required]
    public required IReadOnlyList<EntityMinimalDto> Defenders { get; init; }

    /// <summary>
    ///     The winner of the combat
    /// </summary>
    [Required]
    public required CombatSide Winner { get; init; }

    /// <summary>
    ///     The duration of the combat
    /// </summary>
    [Required]
    public required int Duration { get; init; }
}

static class ArchivedCombatMappingExtensions
{
    public static ArchivedCombatDto ToArchivedCombatDto(this CombatEndedHistoryEntry entry) =>
        new()
        {
            Id = entry.CombatInstanceId.Guid,
            Attackers = entry.Attackers.Select(a => new EntityMinimalDto { Id = a.Id.Guid, Name = a.Name }).ToArray(),
            Defenders = entry.Defenders.Select(a => new EntityMinimalDto { Id = a.Id.Guid, Name = a.Name }).ToArray(),
            Winner = entry.Winner,
            Duration = entry.Turn
        };
}
