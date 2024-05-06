using MediatR;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Core.Gameplay.Interactions;
using RestAdventure.Core.History;
using RestAdventure.Core.Players;
using RestAdventure.Core.Settings;

namespace RestAdventure.Core;

public class GameState
{
    public GameState(IPublisher publisher, GameSettings settings)
    {
        Publisher = publisher;
        Settings = settings;
        History = new GameHistory(this);
        Players = new GamePlayers(this);
        Entities = new GameEntities(this);
        Actions = new GameActions(this);
        Interactions = new GameInteractions(this);
    }


    public GameId Id { get; } = new(Guid.NewGuid());
    public long Tick { get; set; }

    public IPublisher Publisher { get; }
    public GameSettings Settings { get; }
    public GameHistory History { get; }
    public GamePlayers Players { get; }
    public GameEntities Entities { get; }
    public GameActions Actions { get; }
    public GameInteractions Interactions { get; }
}
