using RestAdventure.Core.Combat;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Serialization.Combats;

public class CombatInstanceSnapshot
{
    public CombatInstanceSnapshot(CombatInstanceId id)
    {
        Id = id;
    }

    public CombatInstanceId Id { get; }
    public required Location Location { get; init; }
    public required CombatPhase Phase { get; init; }
    public required int Turn { get; init; }
    public required CombatFormationSnapshot Attackers { get; init; }
    public required CombatFormationSnapshot Defenders { get; init; }
    public required IReadOnlyCollection<CombatEntitySnapshot> AttackerCombatEntities { get; init; }
    public required IReadOnlyCollection<CombatEntitySnapshot> DefenderCombatEntities { get; init; }

    public static CombatInstanceSnapshot Take(CombatInstance combat) =>
        new(combat.Id)
        {
            Location = combat.Location,
            Phase = combat.Phase,
            Turn = combat.Turn,
            Attackers = CombatFormationSnapshot.Take(combat.Attackers),
            Defenders = CombatFormationSnapshot.Take(combat.Defenders),
            AttackerCombatEntities = combat.AttackerCombatEntities.Select(CombatEntitySnapshot.Take).ToArray(),
            DefenderCombatEntities = combat.DefenderCombatEntities.Select(CombatEntitySnapshot.Take).ToArray()
        };
}
