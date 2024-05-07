using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.Interactions.Providers;

namespace RestAdventure.Core.Monsters;

public class MonsterCombatInteractionProvider : IInteractionProvider
{
    readonly MonsterCombatInteraction _cachedMonsterCombatInteraction = new();

    public IEnumerable<Interaction> GetAvailableInteractions(Character character, IGameEntity entity)
    {
        if (entity is not MonsterInstance)
        {
            yield break;
        }

        yield return _cachedMonsterCombatInteraction;
    }
}
