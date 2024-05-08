using RestAdventure.Kernel.Security;

namespace RestAdventure.Core.Players;

/// <summary>
///     A user in a game
/// </summary>
public class Player : IDisposable
{
    public Player(User user)
    {
        User = user;
    }

    public User User { get; }
    public PlayerKnowledge Knowledge { get; } = new();

    public override string ToString() => User.Name;

    public void Dispose()
    {
        Knowledge.Dispose();
        GC.SuppressFinalize(this);
    }
}
