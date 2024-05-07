using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Combat;

public interface IGameEntityWithCombatStatistics : IGameEntity
{
    EntityCombatStatistics Combat { get; }
}
