namespace RestAdventure.Core.Maps.Harvestables;

public class GameHarvestables
{
    readonly Dictionary<HarvestableId, Harvestable> _harvestables = [];

    public IEnumerable<Harvestable> All => _harvestables.Values;

    public void Register(Harvestable harvestable) => _harvestables[harvestable.Id] = harvestable;
    public Harvestable? Get(HarvestableId harvestableId) => _harvestables.GetValueOrDefault(harvestableId);
}

public static class GameHarvestablesExtensions
{
    public static Harvestable Require(this GameHarvestables harvestables, HarvestableId harvestableId) =>
        harvestables.Get(harvestableId) ?? throw new InvalidOperationException($"Could not find harvestable {harvestableId}");
}
