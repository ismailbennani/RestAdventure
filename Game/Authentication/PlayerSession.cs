using RestAdventure.Core.Players;

namespace RestAdventure.Game.Authentication;

class PlayerSession
{
    public PlayerSession(PlayerId playerId, string playerName)
    {
        PlayerName = playerName;
        PlayerId = playerId;
        StartDate = DateTime.Now;
    }

    public PlayerId PlayerId { get; private set; }
    public string PlayerName { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? LastActivityDate { get; set; }
}
