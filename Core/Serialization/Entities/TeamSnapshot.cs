using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Serialization.Entities;

public class TeamSnapshot : ITeam
{
    TeamSnapshot(TeamId id)
    {
        Id = id;
    }

    public TeamId Id { get; }

    public static TeamSnapshot Take(ITeam team) => new(team.Id);
}
