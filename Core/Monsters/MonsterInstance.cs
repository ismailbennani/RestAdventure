using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Gameplay.Interactions;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Monsters;

public record MonterInstanceId(Guid Guid) : GameEntityId(Guid);

public class MonsterInstance : GameEntity<MonterInstanceId>, IGameEntityWithInteractions, IGameEntityWithCombatStatistics
{
    public MonsterInstance(MonsterSpecies species, Location location) : base(new MonterInstanceId(Guid.NewGuid()), species.Name, location)
    {
        Species = species;
        Interactions = new EntityInteractions(new CombatInteraction());
        Combat = new EntityCombatStatistics(Species.Health, Species.Speed, Species.Attack);
    }

    public MonsterSpecies Species { get; }
    public EntityInteractions Interactions { get; }
    public EntityCombatStatistics Combat { get; }
}
