namespace RestAdventure.Core.Entities;

public record TeamId(Guid Guid);

public class Team : ITeam
{
    public TeamId Id { get; } = new(Guid.NewGuid());
}
