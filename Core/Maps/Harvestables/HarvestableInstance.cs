using RestAdventure.Core.Entities;
using RestAdventure.Core.Gameplay.Interactions;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Maps.Harvestables;

public record HarvestableInstanceId(Guid Guid) : GameEntityId(Guid);

/// <summary>
///     Instance of a <see cref="Harvestables.Harvestable" />
/// </summary>
public class HarvestableInstance : GameEntity<HarvestableInstanceId>, IGameEntityWithInteractions
{
    public HarvestableInstance(Harvestable harvestable, Location location) : base(new HarvestableInstanceId(Guid.NewGuid()), harvestable.Name, location)
    {
        Harvestable = harvestable;
        Interactions = new EntityInteractions(new HarvestInteraction());
    }

    /// <summary>
    ///     The harvestable entity that is instantiated
    /// </summary>
    public Harvestable Harvestable { get; }

    /// <summary>
    ///     The interactions
    /// </summary>
    public EntityInteractions Interactions { get; }
}
