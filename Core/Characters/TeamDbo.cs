using Xtensive.Orm;

namespace RestAdventure.Core.Characters;

[HierarchyRoot]
public class TeamDbo : Entity
{
    public TeamDbo(Guid playerId)
    {
        PlayerId = playerId;
    }

    [Key]
    [Field]
    public Guid Id { get; private set; }

    [Field]
    public Guid PlayerId { get; private set; }

    [Field]
    [Association(nameof(CharacterDbo.Team), OnTargetRemove = OnRemoveAction.Clear, OnOwnerRemove = OnRemoveAction.Cascade)]
    public EntitySet<CharacterDbo> Characters { get; private set; }
}
