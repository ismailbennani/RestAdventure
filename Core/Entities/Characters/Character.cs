using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities.Characters.Combats;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Players;
using RestAdventure.Core.Utils;

namespace RestAdventure.Core.Entities.Characters;

public record CharacterId(Guid Guid) : GameEntityId(Guid);

public class Character : GameEntity<CharacterId>, IGameEntityWithInventory, IGameEntityWithJobs, IGameEntityWithCombatCapabilities, IGameEntityWithMovement
{
    public Character(Player player, CharacterClass characterClass, string name) : base(new CharacterId(Guid.NewGuid()), player.Team, name, characterClass.StartLocation)
    {
        Player = player;
        Class = characterClass;
        Health = Class.Health;
        Progression = new ProgressionBar(characterClass.LevelCaps);
        Inventory = new Inventory();
        Jobs = new EntityJobs();
        Movement = new EntityMovement(this);
    }

    public Player Player { get; }

    /// <summary>
    ///     The class of the character
    /// </summary>
    public CharacterClass Class { get; private set; }

    /// <summary>
    ///     The health of the character
    /// </summary>
    public int Health { get; set; }

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
    ///     The movement of the character
    /// </summary>
    public EntityMovement Movement { get; }

    public override async Task KillAsync(Game state) => await TeleportAsync(state, Class.StartLocation);

    public IEnumerable<ICombatEntity> SpawnCombatEntities() => [new CharacterCombatEntity(this)];

    public void DestroyCombatEntities(IEnumerable<ICombatEntity> entities) { }

    public override string ToString() => $"{Class} {Name} ({Player})";

    public override void Dispose()
    {
        Inventory.Dispose();
        Jobs.Dispose();
        GC.SuppressFinalize(this);
    }
}
