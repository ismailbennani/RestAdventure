using RestAdventure.Core.Entities;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Players;

namespace RestAdventure.Core.Characters;

public record CharacterId(Guid Guid) : GameEntityId(Guid);

public class Character : GameEntity<CharacterId>, IGameEntityWithInventory, IGameEntityWithJobs
{
    public Character(Player player, string name, CharacterClass characterClass, Location location) : base(new CharacterId(Guid.NewGuid()), name, location)
    {
        Player = player;
        Class = characterClass;
        Inventory = new Inventory();
        Jobs = new EntityJobs();
    }

    public Player Player { get; }

    /// <summary>
    ///     The class of the character
    /// </summary>
    public CharacterClass Class { get; private set; }

    /// <summary>
    ///     The inventory of the character
    /// </summary>
    public Inventory Inventory { get; private set; }

    /// <summary>
    ///     The jobs of the character
    /// </summary>
    public EntityJobs Jobs { get; private set; }

    public override string ToString() => $"{Class} {Name} ({Player})";
}
