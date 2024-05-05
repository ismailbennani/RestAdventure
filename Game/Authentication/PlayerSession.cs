using RestAdventure.Kernel.Security;

namespace RestAdventure.Game.Authentication;

class PlayerSession
{
    public PlayerSession(UserId userId, string playerName)
    {
        PlayerName = playerName;
        UserId = userId;
        StartDate = DateTime.Now;
    }

    public UserId UserId { get; private set; }
    public string PlayerName { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? LastActivityDate { get; set; }
}
