using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Monsters;

public record MonsterInstanceId(Guid Guid) : GameEntityId(Guid);

public class MonsterInstance : GameEntity<MonsterInstanceId>, IGameEntityWithCombatStatistics, IInteractibleEntity
{
    public MonsterInstance(MonsterSpecies species, int level, Location location) : base(new MonsterInstanceId(Guid.NewGuid()), species.Name, location)
    {
        Species = species;
        Level = level;
        Combat = new EntityCombatStatistics(Species.Health, Species.Speed, Species.Attack);
    }

    public MonsterSpecies Species { get; }
    public int Level { get; }
    public EntityCombatStatistics Combat { get; }

    public bool Disabled { get; private set; }
    public void Enable() => Disabled = false;
    public void Disable() => Disabled = true;
}
