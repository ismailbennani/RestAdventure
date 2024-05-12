using RestAdventure.Core.Players;
using RestAdventure.Core.Resources;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Kernel.Security;

namespace RestAdventure.Core.Serialization;

public class PlayerSnapshot
{
    public PlayerSnapshot(UserId userId)
    {
        UserId = userId;
    }

    public UserId UserId { get; }
    public required TeamSnapshot Team { get; init; }
    public required IReadOnlyCollection<ResourceId> Knowledge { get; init; }

    public static PlayerSnapshot Take(Player player) =>
        new(player.User.Id)
        {
            Team = TeamSnapshot.Take(player.Team),
            Knowledge = player.Knowledge.ToHashSet()
        };
}
