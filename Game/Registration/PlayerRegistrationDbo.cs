using RestAdventure.Core.Players;
using Xtensive.Orm;

namespace RestAdventure.Game.Registration;

[HierarchyRoot]
class PlayerRegistrationDbo : Entity
{
    public PlayerRegistrationDbo(PlayerDbo player)
    {
        Player = player;
        CreationDate = DateTime.Now;
    }

    [Key]
    [Field]
    public Guid ApiKey { get; private set; }

    [Field]
    [Association(OnTargetRemove = OnRemoveAction.Cascade)]
    public PlayerDbo Player { get; private set; }

    [Field]
    public DateTime CreationDate { get; private set; }
}
