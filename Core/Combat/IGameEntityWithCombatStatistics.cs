using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Combat;

public interface IGameEntityWithCombatStatistics : IGameEntity
{
    int Level { get; }
    EntityCombatStatistics Combat { get; }
}
