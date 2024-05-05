using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Maps.Harvestables;

public class GameMapHarvestables
{
    readonly Dictionary<MapHarvestableInstanceId, MapHarvestableInstance> _harvestables = new();

    public IReadOnlyCollection<MapHarvestableInstance> All => _harvestables.Values;

    public void Register(MapHarvestableInstance harvestable) => _harvestables[harvestable.Id] = harvestable;
    public MapHarvestableInstance? Get(MapHarvestableInstanceId harvestableId) => _harvestables.GetValueOrDefault(harvestableId);
    public IEnumerable<MapHarvestableInstance> GetAt(MapLocation location) => _harvestables.Values.Where(h => h.Location == location);
}

public static class GameHarvestablesExtensions
{
    public static MapHarvestableInstance Require(this GameMapHarvestables harvestables, MapHarvestableInstanceId mapHarvestableId) =>
        harvestables.Get(mapHarvestableId) ?? throw new InvalidOperationException($"Could not find harvestable {mapHarvestableId}");
}
