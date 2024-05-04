using System.Text.Json;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Game.Settings;

namespace RestAdventure.Core;

public class GameService
{
    readonly CharacterActionsService _characterActionsService;
    readonly ILogger<GameService> _logger;

    GameState? _gameState;

    public GameService(CharacterActionsService characterActionsService, ILogger<GameService> logger)
    {
        _characterActionsService = characterActionsService;
        _logger = logger;
    }

    public async Task<GameState> NewGameAsync(GameSettings settings)
    {
        _gameState = new GameState(settings);

        _logger.LogInformation("Game state has been initialized with settings: {settingsJson}.", JsonSerializer.Serialize(settings));

        return _gameState;
    }

    public async Task<GameState> LoadGameAsync() => throw new NotImplementedException();

    public GameState RequireGameState()
    {
        if (_gameState == null)
        {
            throw new InvalidOperationException($"No game has been loaded. Please call {nameof(NewGameAsync)} or {nameof(LoadGameAsync)}.");
        }

        return _gameState;
    }

    public long Tick()
    {
        GameState state = RequireGameState();

        state.Tick++;

        _characterActionsService.ResolveActions(state);

        return state.Tick;
    }
}
