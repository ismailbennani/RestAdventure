﻿using RestAdventure.Core.Conditions.Characters;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Maps.Harvestables;

public record HarvestableId(Guid Guid) : ResourceId(Guid);

/// <summary>
///     A harvestable entity. Harvestable entities are interactible entities found in the world. They contain items that the player can harvest.
///     There should be only one instance of this class per harvestable entity. It stores all the meta data about the entity: its name, description, etc...
///     The materialization of the entity in the game world is <see cref="HarvestableInstance" />.
/// </summary>
public class Harvestable : GameResource<HarvestableId>
{
    public Harvestable() : base(new HarvestableId(Guid.NewGuid())) { }

    /// <summary>
    ///     The name of the harvestable entity
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     The description of the harvestable entity
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     The condition that a character should fulfill to harvest the entity
    /// </summary>
    public ICharacterCondition? HarvestCondition { get; init; }

    /// <summary>
    ///     The number of ticks it takes the player to harvest the entity
    /// </summary>
    public required int HarvestDuration { get; init; }

    /// <summary>
    ///     The items that should be given to the character when they are done harvesting
    /// </summary>
    public required IReadOnlyCollection<ItemStack> Items { get; init; }

    /// <summary>
    ///     The experience that should be given to the player when they are done harvesting
    /// </summary>
    public required IReadOnlyCollection<JobExperienceStack> Experience { get; init; }

    public override string ToString() => $"Harvestable {Name} ({Id})";
}
