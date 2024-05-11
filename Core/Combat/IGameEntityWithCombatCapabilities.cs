using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Combat;

public interface IGameEntityWithCombatCapabilities : IGameEntity
{
    IEnumerable<ICombatEntity> SpawnCombatEntities();
    void DestroyCombatEntities(IEnumerable<ICombatEntity> entities);
}
