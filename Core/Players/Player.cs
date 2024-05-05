using RestAdventure.Kernel.Security;

namespace RestAdventure.Core.Players;

/// <summary>
///     A user in a game
/// </summary>
public class Player
{
    public Player(User user)
    {
        User = user;
    }

    public User User { get; }
    public PlayerKnowledge Knowledge { get; } = new();

    public override string ToString() => $"{User}";
}
