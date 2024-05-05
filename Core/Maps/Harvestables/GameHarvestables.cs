namespace RestAdventure.Core.Maps.Harvestables;

public class GameHarvestables
{
    readonly Dictionary<MapHarvestableId, MapHarvestable> _harvestables = [];

    public IEnumerable<MapHarvestable> All => _harvestables.Values;

    public void Register(MapHarvestable harvestable) => _harvestables[harvestable.Id] = harvestable;
    public MapHarvestable? Get(MapHarvestableId harvestableId) => _harvestables.GetValueOrDefault(harvestableId);
}

public static class GameHarvestablesExtensions
{
    public static MapHarvestable Require(this GameHarvestables harvestables, MapHarvestableId harvestableId) =>
        harvestables.Get(harvestableId) ?? throw new InvalidOperationException($"Could not find harvestable {harvestableId}");
}
