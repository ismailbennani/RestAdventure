using RestAdventure.Core.Entities;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Maps.Harvestables;

public record HarvestableInstanceId(Guid Guid) : GameEntityId(Guid);

/// <summary>
///     Instance of a <see cref="Harvestables.Harvestable" />
/// </summary>
public class HarvestableInstance : GameEntity<HarvestableInstanceId>
{
    public HarvestableInstance(Harvestable harvestable, Location location) : base(new HarvestableInstanceId(Guid.NewGuid()), harvestable.Name, location)
    {
        Harvestable = harvestable;
    }

    /// <summary>
    ///     The harvestable entity that is instantiated
    /// </summary>
    public Harvestable Harvestable { get; }
}
