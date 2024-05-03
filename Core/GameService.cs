using Microsoft.Extensions.Logging;
using Xtensive.Orm;

namespace RestAdventure.Core;

public class GameService
{
    readonly ILogger<GameService> _logger;

    public GameService(ILogger<GameService> logger)
    {
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

        return state;
    }
}
