﻿using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Core.Serialization.Entities;

namespace RestAdventure.Core.Jobs;

/// <summary>
/// </summary>
public class JobHarvest
{
    /// <summary>
    ///     The unique name of this harvest
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     The min level required for the harvest
    /// </summary>
    public required int Level { get; init; }

    /// <summary>
    ///     If set, this harvest requires the use of a tool of the specified category
    /// </summary>
    public required ItemCategory? Tool { get; init; }

    /// <summary>
    ///     The number of ticks it takes to perform the harvest
    /// </summary>
    public required int HarvestDuration { get; init; }

    /// <summary>
    ///     The objects that can be harvested
    /// </summary>
    public required IReadOnlyCollection<StaticObject> Targets { get; init; }

    /// <summary>
    ///     The items that should be given to the character when they are done harvesting
    /// </summary>
    public required IReadOnlyCollection<ItemStack> Items { get; init; }

    /// <summary>
    ///     The experience that should be given to the player when they are done harvesting
    /// </summary>
    public required int Experience { get; init; }
}

public static class JobHarvestMappingExtensions
{
    public static bool CanTarget(this JobHarvest harvest, StaticObjectInstance instance) => harvest.Targets.Contains(instance.Object);
    public static bool CanTarget(this JobHarvest harvest, StaticObjectInstanceSnapshot instance) => harvest.Targets.Contains(instance.Object);
}
