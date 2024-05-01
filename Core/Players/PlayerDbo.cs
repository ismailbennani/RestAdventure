using Xtensive.Orm;

namespace Core.Players;

[HierarchyRoot]
public class PlayerDbo : Entity
{
    public PlayerDbo(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     The ID of the player
    /// </summary>
    [Key]
    [Field]
    public Guid Id { get; private set; }

    /// <summary>
    ///     The name of the player
    /// </summary>
    [Field]
    public string Name { get; set; }
}
