using System.Text.Json;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Core.Settings;

namespace RestAdventure.Core;

public class GameService
{
    readonly CharacterActionsService _characterActionsService;
    readonly ILogger<GameService> _logger;

    GameContent? _gameContent;
    GameState? _gameState;

    public GameService(CharacterActionsService characterActionsService, ILogger<GameService> logger)
    {
        _characterActionsService = characterActionsService;
        _logger = logger;
    }

    public GameState NewGame(GameContent content, GameSettings settings)
    {
        _gameContent = content;
        _gameState = new GameState(settings);

        _logger.LogInformation("Game state has been initialized with settings: {settingsJson}.", JsonSerializer.Serialize(settings));

        return _gameState;
    }

    public GameState LoadGame() => throw new NotImplementedException();

    public GameState RequireGameState()
    {
        if (_gameState == null)
        {
            throw new InvalidOperationException($"No game has been loaded. Please call {nameof(NewGame)} or {nameof(LoadGame)}.");
        }

        return _gameState;
    }

    public GameContent RequireGameContent()
    {
        if (_gameContent == null)
        {
            throw new InvalidOperationException($"No game has been loaded. Please call {nameof(NewGame)} or {nameof(LoadGame)}.");
        }

        return _gameContent;
    }

    public long Tick()
    {
        GameContent content = RequireGameContent();
        GameState state = RequireGameState();

        state.Tick++;

        _characterActionsService.ResolveActions(content, state);

        return state.Tick;
    }
}
