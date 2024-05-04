using RestAdventure.Core.Characters;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Players;
using RestAdventure.Game.Settings;

namespace RestAdventure.Core;

public class GameState
{
    public GameState(GameSettings settings)
    {
        Settings = settings;
        Players = new GamePlayersState(this);
        Map = new GameMapState(this);
        Characters = new GameCharactersState(this);
    }


    public GameId Id { get; } = new(Guid.NewGuid());
    public long Tick { get; set; }

    public GameSettings Settings { get; }
    public GamePlayersState Players { get; }
    public GameMapState Map { get; }
    public GameCharactersState Characters { get; }
}
