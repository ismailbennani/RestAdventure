using MediatR;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Actions;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Pve;
using RestAdventure.Core.Content;
using RestAdventure.Core.Entities;
using RestAdventure.Core.History;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Players;

namespace RestAdventure.Core;

public class GameState : IDisposable
{
    public GameState(GameSettings settings, GameContent content, IPublisher publisher, ILoggerFactory loggerFactory)
    {
        Publisher = publisher;
        Settings = settings;
        Content = content;
        History = new GameHistory();
        Players = new GamePlayers(publisher);
        Entities = new GameEntities(publisher);
        Actions = new GameActions(
            this,
            [new PveCombatActionsProvider(loggerFactory), new HarvestActionsProvider(), new MoveActionsProvider()],
            loggerFactory.CreateLogger<GameActions>()
        );
        Combats = new GameCombats(this, loggerFactory.CreateLogger<GameCombats>());
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
    public GameCombats Combats { get; }

    public void Dispose()
    {
        Players.Dispose();
        Entities.Dispose();
        Combats.Dispose();
        GC.SuppressFinalize(this);
    }
}
