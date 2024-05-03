using Xtensive.Orm;

namespace RestAdventure.Game.Registration;

[HierarchyRoot]
class PlayerRegistrationDbo : Entity
{
    public PlayerRegistrationDbo(Guid playerId, string playerName)
    {
        Player = new PlayerIdentityDbo(playerId, playerName);
        CreationDate = DateTime.Now;
    }

    [Key]
    [Field]
    public Guid ApiKey { get; private set; }

    [Field]
    public PlayerIdentityDbo Player { get; private set; }

    [Field]
    public DateTime CreationDate { get; private set; }
}
