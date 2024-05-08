using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Monsters;

public record MonsterInstanceId(Guid Guid) : GameEntityId(Guid);

public class MonsterInstance : GameEntity<MonsterInstanceId>, IGameEntityWithCombatStatistics
{
    public MonsterInstance(MonsterSpecies species, int level, Location location) : base(new MonsterInstanceId(Guid.NewGuid()), species.Name, location)
    {
        Species = species;
        Level = level;
        CombatStatistics = new EntityCombatStatistics(Species.Health, Species.Speed, Species.Attack);
    }

    public MonsterSpecies Species { get; }
    public int Level { get; }
    public EntityCombatStatistics CombatStatistics { get; }
    public CombatEntityKind CombatEntityKind => CombatEntityKind.Monster;

    public CombatInPreparation? CombatInPreparation { get; set; }
    public CombatInstance? Combat { get; set; }
}
