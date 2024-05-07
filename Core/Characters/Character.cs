using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Players;
using RestAdventure.Core.Utils;

namespace RestAdventure.Core.Characters;

public record CharacterId(Guid Guid) : GameEntityId(Guid);

public class Character : GameEntity<CharacterId>, IGameEntityWithInventory, IGameEntityWithJobs, IGameEntityWithCombatStatistics
{
    public Character(Player player, CharacterClass characterClass, string name) : base(new CharacterId(Guid.NewGuid()), name, characterClass.StartLocation)
    {
        Player = player;
        Class = characterClass;
        Progression = new ProgressionBar(characterClass.LevelCaps);
        Inventory = new Inventory();
        Jobs = new EntityJobs();
        Combat = new EntityCombatStatistics(10, 100, 10);
    }

    public Player Player { get; }

    /// <summary>
    ///     The class of the character
    /// </summary>
    public CharacterClass Class { get; private set; }

    /// <summary>
    ///     The progression of the character
    /// </summary>
    public ProgressionBar Progression { get; private set; }

    /// <summary>
    ///     The inventory of the character
    /// </summary>
    public Inventory Inventory { get; private set; }

    /// <summary>
    ///     The jobs of the character
    /// </summary>
    public EntityJobs Jobs { get; private set; }

    /// <summary>
    ///     The combat statistics of the character
    /// </summary>
    public EntityCombatStatistics Combat { get; private set; }

    public override string ToString() => $"{Class} {Name} ({Player})";

    public override void Dispose()
    {
        Inventory.Dispose();
        Jobs.Dispose();
        GC.SuppressFinalize(this);
    }
}
