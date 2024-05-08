using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.Interactions.Providers;

namespace RestAdventure.Core.Monsters;

public class MonsterCombatInteractionProvider : IInteractionProvider
{
    readonly GameService _gameService;
    MonsterCombatInteraction? _cachedMonsterCombatInteraction;

    public MonsterCombatInteractionProvider(GameService gameService)
    {
        _gameService = gameService;
    }

    public IEnumerable<Interaction> GetAvailableInteractions(Character character, IGameEntity entity)
    {
        if (entity is not MonsterInstance)
        {
            yield break;
        }

        GameState state = _gameService.RequireGameState();
        yield return _cachedMonsterCombatInteraction ??= new MonsterCombatInteraction(state.Combats);
    }
}
