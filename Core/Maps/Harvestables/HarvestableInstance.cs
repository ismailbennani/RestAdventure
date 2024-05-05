using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Maps.Harvestables;

public record HarvestableInstanceId(Guid Guid) : ResourceId(Guid);

/// <summary>
///     Instance of a <see cref="Harvestables.Harvestable" />
/// </summary>
public class HarvestableInstance
{
    /// <summary>
    ///     The unique ID of the instance
    /// </summary>
    public HarvestableInstanceId Id { get; } = new(Guid.NewGuid());

    /// <summary>
    ///     The harvestable entity that is instantiated
    /// </summary>
    public required Harvestable Harvestable { get; init; }

    /// <summary>
    ///     The location of the harvestable instance
    /// </summary>
    public required Location Location { get; init; }
}
