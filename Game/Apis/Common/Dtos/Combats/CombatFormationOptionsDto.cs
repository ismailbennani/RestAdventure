using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat.Options;

namespace RestAdventure.Game.Apis.Common.Dtos.Combats;

/// <summary>
///     Combat formation options
/// </summary>
public class CombatFormationOptionsDto
{
    /// <summary>
    ///     The accessibility of the formation
    /// </summary>
    [Required]
    public required CombatFormationAccessibility Accessibility { get; init; }
}

static class CombatFormationOptionsMappingExtensions
{
    public static CombatFormationOptionsDto ToDto(this CombatFormationOptions options) =>
        new()
        {
            Accessibility = options.Accessibility
        };

    public static CombatFormationOptions ToBusiness(this CombatFormationOptionsDto options) =>
        new()
        {
            Accessibility = options.Accessibility
        };
}
