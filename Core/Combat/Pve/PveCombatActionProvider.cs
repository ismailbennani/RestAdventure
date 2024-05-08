using Microsoft.Extensions.Logging;
using RestAdventure.Core.Actions.Providers;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Monsters;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatActionProvider : IActionProvider
{
    readonly GameService _gameService;
    readonly ILoggerFactory _loggerFactory;

    public PveCombatActionProvider(GameService gameService, ILoggerFactory loggerFactory)
    {
        _gameService = gameService;
        _loggerFactory = loggerFactory;
    }

    public IEnumerable<Action> GetActions(Character character)
    {
        GameState state = _gameService.RequireGameState();

        IEnumerable<MonsterInstance> monsters = state.Entities.AtLocation<MonsterInstance>(character.Location);
        foreach (MonsterInstance monster in monsters)
        {
            yield return new PveCombatAction([monster], _loggerFactory.CreateLogger<PveCombatAction>());
        }
    }
}
