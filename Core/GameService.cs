using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Settings;
using RestAdventure.Core.Simulation.Notifications;

namespace RestAdventure.Core;

public class GameService
{
    readonly IPublisher _publisher;
    readonly ILoggerFactory _loggerFactory;
    readonly ILogger<GameService> _logger;

    GameContent? _gameContent;
    GameState? _gameState;

    public GameService(IPublisher publisher, ILoggerFactory loggerFactory)
    {
        _publisher = publisher;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<GameService>();
    }

    public GameState NewGame(GameContent content, GameSettings settings)
    {
        _gameContent = content;
        _gameState = new GameState(settings, content, _publisher, _loggerFactory);

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
        GameState state = RequireGameState();

        state.Tick++;

        await state.Actions.ResolveActionsAsync(state);
        await state.Combats.ResolveCombatsAsync(state);

        await _publisher.Publish(new GameTick { GameState = state });

        return state.Tick;
    }
}
