namespace GameApi.Authentication;

class PlayerSession
{
    public PlayerSession(Guid playerId, string playerName)
    {
        PlayerName = playerName;
        PlayerId = playerId;
        StartDate = DateTime.Now;
    }

    public Guid PlayerId { get; private set; }
    public string PlayerName { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? LastActivityDate { get; set; }
}
