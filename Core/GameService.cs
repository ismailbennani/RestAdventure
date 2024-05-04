using Microsoft.Extensions.Logging;
using RestAdventure.Core.Gameplay.Actions;
using Xtensive.Orm;

namespace RestAdventure.Core;

public class GameService
{
    readonly CharacterActionsService _characterActionsService;
    readonly ILogger<GameService> _logger;

    public GameService(CharacterActionsService characterActionsService, ILogger<GameService> logger)
    {
        _characterActionsService = characterActionsService;
        _logger = logger;
    }

    public async Task<GameStateDbo> GetStateAsync()
    {
        GameStateDbo state = await GetOrCreateStateAsync();
        return state;
    }

    public async Task<long> TickAsync()
    {
        GameStateDbo state = await GetOrCreateStateAsync();
        return ++state.Tick;
    }

    async Task<GameStateDbo> GetOrCreateStateAsync()
    {
        GameStateDbo? state = await Query.All<GameStateDbo>().SingleOrDefaultAsync();
        if (state == null)
        {
            state = new GameStateDbo();
            _logger.LogInformation("Game state has been initialized.");
        }

        await _characterActionsService.ResolveActionsAsync(state);

        return state;
    }
}
