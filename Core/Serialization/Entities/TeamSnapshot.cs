using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Serialization.Entities;

public class TeamSnapshot
{
    TeamSnapshot(TeamId id)
    {
        Id = id;
    }

    public TeamId Id { get; }

    public static TeamSnapshot Take(Team team) => new(team.Id);
}
