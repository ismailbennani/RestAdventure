using RestAdventure.Core.Combat;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Entities.Monsters;

public record MonsterInstanceId(Guid Guid) : GameEntityId(Guid);

/// <summary>
///     A monster in a combat. Individual monsters are only spawned for combats, they are destroyed once the combat is over.
/// </summary>
public class MonsterCombatInstance : GameEntity<MonsterInstanceId>, IGameEntityWithCombatStatistics
{
    public MonsterCombatInstance(Team team, MonsterSpecies species, int level, Location location) : base(new MonsterInstanceId(Guid.NewGuid()), team, species.Name, location)
    {
        Species = species;
        Level = level;
        CombatStatistics = new EntityCombatStatistics(Species.Health, Species.Speed, Species.Attack);
    }

    public MonsterSpecies Species { get; }
    public int Level { get; }
    public EntityCombatStatistics CombatStatistics { get; }
    public CombatEntityKind CombatEntityKind => CombatEntityKind.Monster;
}
