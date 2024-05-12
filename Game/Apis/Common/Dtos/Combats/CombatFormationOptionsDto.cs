using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat.Options;
using RestAdventure.Core.Serialization.Combats;

namespace RestAdventure.Game.Apis.Common.Dtos.Combats;

/// <summary>
///     Combat formation options
/// </summary>
public class CombatFormationOptionsDto
{
    /// <summary>
    ///     The accessibility of the combat formation
    /// </summary>
    [Required]
    public required CombatFormationAccessibility Accessibility { get; init; }

    /// <summary>
    ///     The max number of entities in the formation
    /// </summary>
    [Required]
    public required int Slots { get; init; }
}

static class CombatFormationOptionsMappingExtensions
{
    public static CombatFormationOptionsDto ToDto(this CombatFormationOptionsSnapshot options) =>
        new()
        {
            Accessibility = options.Accessibility,
            Slots = options.MaxCount
        };
}
