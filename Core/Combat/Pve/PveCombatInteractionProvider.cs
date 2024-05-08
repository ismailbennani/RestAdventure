using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.Interactions.Providers;
using RestAdventure.Core.Monsters;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatInteractionProvider : IInteractionProvider
{
    readonly GameService _gameService;
    PveCombatInteraction? _cachedMonsterCombatInteraction;

    public PveCombatInteractionProvider(GameService gameService)
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
        yield return _cachedMonsterCombatInteraction ??= new PveCombatInteraction(state.Combats);
    }
}
