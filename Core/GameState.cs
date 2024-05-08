using MediatR;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Actions;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities;
using RestAdventure.Core.History;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.Players;
using RestAdventure.Core.Settings;

namespace RestAdventure.Core;

public class GameState : IDisposable
{
    public GameState(GameSettings settings, GameContent content, IPublisher publisher, ILoggerFactory loggerFactory)
    {
        Publisher = publisher;
        Settings = settings;
        Content = content;
        History = new GameHistory();
        Players = new GamePlayers(this);
        Entities = new GameEntities(this);
        Actions = new GameActions(this, loggerFactory.CreateLogger<GameActions>());
        Interactions = new GameInteractions(this);
        Combats = new GameCombats(settings, publisher, loggerFactory.CreateLogger<GameCombats>());
    }

    public IPublisher Publisher { get; }
    public GameSettings Settings { get; }
    public GameContent Content { get; }

    public GameId Id { get; } = new(Guid.NewGuid());
    public long Tick { get; set; }

    public GameHistory History { get; }
    public GamePlayers Players { get; }
    public GameEntities Entities { get; }
    public GameActions Actions { get; }
    public GameInteractions Interactions { get; }
    public GameCombats Combats { get; }

    public void Dispose()
    {
        Players.Dispose();
        Entities.Dispose();
        Combats.Dispose();
        GC.SuppressFinalize(this);
    }
}
