using MediatR;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Players;
using RestAdventure.Core.Settings;

namespace RestAdventure.Core;

public class GameState
{
    public GameState(IPublisher publisher, GameSettings settings)
    {
        Publisher = publisher;
        Settings = settings;
        Players = new GamePlayers(this);
        Characters = new GameCharacters(this);
    }


    public GameId Id { get; } = new(Guid.NewGuid());
    public long Tick { get; set; }

    public IPublisher Publisher { get; }
    public GameSettings Settings { get; }
    public GamePlayers Players { get; }
    public GameCharacters Characters { get; }
}
