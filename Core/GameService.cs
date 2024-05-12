using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Plugins;

namespace RestAdventure.Core;

public class GameService
{
    readonly IPublisher _publisher;
    readonly ILoggerFactory _loggerFactory;
    readonly ILogger<GameService> _logger;

    GameState? _gameState;

    public GameService(IPublisher publisher, ILoggerFactory loggerFactory)
    {
        _publisher = publisher;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<GameService>();
    }

    public async Task<GameState> NewGameAsync(Scenario scenario, GameSettings settings)
    {
        GameContent gameContent = scenario.ToGameContent();
        _gameState = new GameState(settings, gameContent, _publisher, _loggerFactory);
        await _gameState.Simulation.StartAsync();

        _logger.LogInformation("Game state has been initialized with settings: {settingsJson}.", JsonSerializer.Serialize(settings));

        return _gameState;
    }

    public GameState LoadGame() => throw new NotImplementedException();

    public GameState RequireGameState()
    {
        if (_gameState == null)
        {
            throw new InvalidOperationException($"No game has been loaded. Please call {nameof(NewGameAsync)} or {nameof(LoadGame)}.");
        }

        return _gameState;
    }

    public GameContent RequireGameContent() => RequireGameState().Content;

    public async Task<long> TickAsync()
    {
        GameState state = RequireGameState();

        await state.Simulation.TickAsync();

        return state.Tick;
    }
}
