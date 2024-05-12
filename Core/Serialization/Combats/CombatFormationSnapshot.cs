using RestAdventure.Core.Combat;
using RestAdventure.Core.Serialization.Entities;

namespace RestAdventure.Core.Serialization.Combats;

public class CombatFormationSnapshot
{
    public required IReadOnlyCollection<GameEntitySnapshot> Entities { get; init; }
    public required CombatFormationOptionsSnapshot Options { get; init; }

    public static CombatFormationSnapshot Take(CombatFormation formation) =>
        new()
        {
            Entities = formation.Entities.Select(GameEntitySnapshot.Take).ToArray(),
            Options = CombatFormationOptionsSnapshot.Take(formation.Options)
        };
}
