using Xtensive.Orm;

namespace RestAdventure.Game.Registration;

class PlayerIdentityDbo : Structure
{
    public PlayerIdentityDbo(Guid id, string name)
    {
        Name = name;
        Id = id;
    }

    [Field]
    public Guid Id { get; private set; }

    [Field]
    public string Name { get; set; }
}
