using RestAdventure.Core.Combat;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Players;
using RestAdventure.Core.Utils;

namespace RestAdventure.Core.Entities.Characters;

public record CharacterId(Guid Guid) : GameEntityId(Guid);

public class Character : GameEntity<CharacterId>, IGameEntityWithInventory, IGameEntityWithJobs, IGameEntityWithCombatStatistics, IGameEntityWithMovement
{
    public Character(Player player, CharacterClass characterClass, string name) : base(player.Team, new CharacterId(Guid.NewGuid()), name, characterClass.StartLocation)
    {
        Player = player;
        Class = characterClass;
        Progression = new ProgressionBar(characterClass.LevelCaps);
        Inventory = new Inventory();
        Jobs = new EntityJobs();
        CombatStatistics = new EntityCombatStatistics(10, 100, 1);
        Movement = new EntityMovement(this);
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

    public int Level => Progression.Level;

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
    public EntityCombatStatistics CombatStatistics { get; private set; }

    public CombatEntityKind CombatEntityKind => CombatEntityKind.Character;

    /// <summary>
    ///     The movement of the character
    /// </summary>
    public EntityMovement Movement { get; }

    public override string ToString() => $"{Class} {Name} ({Player})";

    public override void Dispose()
    {
        Inventory.Dispose();
        Jobs.Dispose();
        GC.SuppressFinalize(this);
    }
}
