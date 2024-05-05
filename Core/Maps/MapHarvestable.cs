using RestAdventure.Kernel;

namespace RestAdventure.Core.Maps;

public record MapHarvestableId(Guid Guid) : Id(Guid);

/// <summary>
///     A harvestable entity. Harvestable entities are interactible entities found in the world. They contain items that the player can harvest.
///     There should be only one instance of this class per harvestable entity. It stores all the meta data about the entity: its name, description, etc...
///     The materialization of the entity in the game world is <see cref="MapHarvestableInstance" />.
/// </summary>
public class MapHarvestable
{
    /// <summary>
    ///     The unique ID of this harvestable entity
    /// </summary>
    public MapHarvestableId Id { get; } = new(Guid.NewGuid());

    /// <summary>
    ///     The name of the harvestable entity
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     The description of the harvestable entity
    /// </summary>
    public string? Description { get; init; }
}
