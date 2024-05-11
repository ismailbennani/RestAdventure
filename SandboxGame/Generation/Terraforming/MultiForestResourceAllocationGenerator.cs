using RestAdventure.Core.Entities.StaticObjects;
using SandboxGame.Generation.Partitioning;
using SandboxGame.Generation.Shaping;
using SandboxGame.Generation.Zoning;

namespace SandboxGame.Generation.Terraforming;

public class MultiForestResourceAllocationGenerator : ResourceAllocationGenerator
{
    public MultiForestResourceAllocationGenerator(ForestResourceAllocationGenerator forestResourceAllocationGenerator, int repeat)
    {
        ForestResourceAllocationGenerator = forestResourceAllocationGenerator;
        Repeat = repeat;
    }

    public ForestResourceAllocationGenerator ForestResourceAllocationGenerator { get; }
    public int Repeat { get; }

    public override IReadOnlyDictionary<(int X, int Y), IReadOnlyCollection<(StaticObject Object, double Count)>> Generate(
        Land land,
        Partition partition,
        IReadOnlyList<Zone> zones
    )
    {
        Dictionary<(int X, int Y), IReadOnlyCollection<(StaticObject Object, double Count)>> result = new();

        for (int i = 0; i < Repeat; i++)
        {
            IReadOnlyDictionary<(int X, int Y), IReadOnlyCollection<(StaticObject Object, double Count)>> newForest = ForestResourceAllocationGenerator.Generate(
                land,
                partition,
                zones
            );
            result = Combine(result, newForest);
        }

        return result;
    }

    static Dictionary<(int X, int Y), IReadOnlyCollection<(StaticObject Object, double Count)>> Combine(
        Dictionary<(int X, int Y), IReadOnlyCollection<(StaticObject Object, double Count)>> old,
        IReadOnlyDictionary<(int X, int Y), IReadOnlyCollection<(StaticObject Object, double Count)>> newResults
    )
    {
        foreach (((int X, int Y) location, IReadOnlyCollection<(StaticObject Object, double Count)> allocation) in newResults)
        {
            if (old.TryAdd(location, allocation))
            {
                continue;
            }

            Dictionary<StaticObject, double> dict = old[location].ToDictionary(oc => oc.Object, oc => oc.Count);
            foreach ((StaticObject Object, double Count) oc in allocation)
            {
                if (!dict.TryAdd(oc.Object, oc.Count))
                {
                    dict[oc.Object] += oc.Count;
                }
            }
            old[location] = dict.Select(kv => (kv.Key, kv.Value)).ToArray();
        }

        return old;
    }
}
