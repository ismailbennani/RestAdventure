using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Plugins;
using RestAdventure.Core.Serialization;

namespace RestAdventure.Core;

public class GameService
{
    readonly IPublisher _publisher;
    readonly ILoggerFactory _loggerFactory;
    readonly ILogger<GameService> _logger;

    Game? _game;
    GameSnapshot? _snapshot;
    readonly object _snapshotLock = new();

    public GameService(IPublisher publisher, ILoggerFactory loggerFactory)
    {
        _publisher = publisher;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<GameService>();
    }

    public async Task<Game> NewGameAsync(Scenario scenario, GameSettings settings)
    {
        GameContent gameContent = scenario.ToGameContent();
        _game = new Game(settings, gameContent, _publisher, _loggerFactory);
        await _game.Simulation.StartAsync();

        _logger.LogInformation("Game state has been initialized with settings: {settingsJson}.", JsonSerializer.Serialize(settings));

        lock (_snapshotLock)
        {
            _snapshot = GameSnapshot.Take(_game);
        }

        return _game;
    }

    public Game LoadGame() => throw new NotImplementedException();

    public Game RequireGame()
    {
        if (_game == null)
        {
            throw new InvalidOperationException($"No game has been loaded. Please call {nameof(NewGameAsync)} or {nameof(LoadGame)}.");
        }

        return _game;
    }

    public GameContent RequireGameContent() => RequireGame().Content;

    public GameSnapshot GetLastSnapshot()
    {
        lock (_snapshotLock)
        {
            if (_snapshot == null)
            {
                throw new InvalidOperationException($"No game has been loaded. Please call {nameof(NewGameAsync)} or {nameof(LoadGame)}.");
            }

            return _snapshot;
        }
    }

    public async Task<long> TickAsync()
    {
        Game game = RequireGame();
        await game.Simulation.TickAsync();

        lock (_snapshotLock)
        {
            _snapshot = GameSnapshot.Take(game);
        }

        return game.Tick;
    }
}
