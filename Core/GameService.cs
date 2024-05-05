using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Core.Settings;
using RestAdventure.Core.Simulation.Notifications;

namespace RestAdventure.Core;

public class GameService
{
    readonly IPublisher _publisher;
    readonly CharacterActionsService _characterActionsService;
    readonly ILogger<GameService> _logger;

    GameContent? _gameContent;
    GameState? _gameState;

    public GameService(IPublisher publisher, CharacterActionsService characterActionsService, ILogger<GameService> logger)
    {
        _publisher = publisher;
        _characterActionsService = characterActionsService;
        _logger = logger;
    }

    public GameState NewGame(GameContent content, GameSettings settings)
    {
        _gameContent = content;
        _gameState = new GameState(_publisher, settings);

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

    public async Task<long> TickAsync()
    {
        GameContent content = RequireGameContent();
        GameState state = RequireGameState();

        state.Tick++;

        _characterActionsService.ResolveActions(content, state);
        await state.Interactions.ResolveInteractionsAsync(content, state);

        await _publisher.Publish(new GameTick { GameState = state });

        return state.Tick;
    }
}
