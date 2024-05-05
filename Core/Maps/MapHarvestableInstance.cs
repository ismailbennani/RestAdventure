using RestAdventure.Kernel;

namespace RestAdventure.Core.Maps;

public record MapHarvestableInstanceId(Guid Guid) : Id(Guid);

/// <summary>
///     Instance of a <see cref="MapHarvestable" />
/// </summary>
public class MapHarvestableInstance
{
    /// <summary>
    ///     The unique ID of the instance
    /// </summary>
    public MapHarvestableInstanceId Id { get; } = new(Guid.NewGuid());

    /// <summary>
    ///     The harvestable entity that is instantiated
    /// </summary>
    public required MapHarvestable Harvestable { get; init; }

    /// <summary>
    ///     The location of the harvestable instance
    /// </summary>
    public required MapLocation Location { get; init; }
}
