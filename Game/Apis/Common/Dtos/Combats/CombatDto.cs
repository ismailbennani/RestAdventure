using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Serialization.Combats;

namespace RestAdventure.Game.Apis.Common.Dtos.Combats;

/// <summary>
///     Combat base
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$phase", IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(CombatInPreparationDto), "preparation")]
[JsonDerivedType(typeof(OngoingCombatDto), "ongoing")]
public abstract class CombatDto
{
    /// <summary>
    ///     The unique ID of the combat instance
    /// </summary>
    [Required]
    public required Guid Id { get; init; }
}

static class CombatMappingExtensions
{
    public static CombatDto ToDto(this CombatInstanceSnapshot combat)
    {
        switch (combat.Phase)
        {
            case CombatPhase.Preparation:
                return combat.ToCombatInPreparation();
            case CombatPhase.Combat:
            case CombatPhase.End:
                return combat.ToOngoingCombatDto();
            default:
                throw new ArgumentOutOfRangeException(nameof(combat.Phase), combat.Phase, null);
        }
    }
}
