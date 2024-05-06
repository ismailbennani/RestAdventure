using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Maps.Harvestables;

public class GameHarvestableInstances
{
    readonly Dictionary<HarvestableInstanceId, HarvestableInstance> _harvestables = new();

    public IReadOnlyCollection<HarvestableInstance> All => _harvestables.Values;

    public void Register(HarvestableInstance harvestable) => _harvestables[harvestable.Id] = harvestable;
    public HarvestableInstance? Get(HarvestableInstanceId harvestableId) => _harvestables.GetValueOrDefault(harvestableId);
    public IEnumerable<HarvestableInstance> AtLocation(Location location) => _harvestables.Values.Where(h => h.Location == location);
}

public static class GameMapHarvestablesExtensions
{
    public static HarvestableInstance Require(this GameHarvestableInstances harvestables, HarvestableInstanceId harvestableId) =>
        harvestables.Get(harvestableId) ?? throw new InvalidOperationException($"Could not find harvestable {harvestableId}");
}
