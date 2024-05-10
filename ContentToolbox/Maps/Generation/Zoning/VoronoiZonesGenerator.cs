using ContentToolbox.Maps.Generation.LandGeneration;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Extensions;

namespace ContentToolbox.Maps.Generation.Zoning;

public class VoronoiZonesGenerator : ZonesGenerator
{
    readonly ILogger<VoronoiZonesGenerator> _logger;

    public VoronoiZonesGenerator(int zonesCount, ILogger<VoronoiZonesGenerator> logger)
    {
        _logger = logger;
        ZonesCount = zonesCount;
    }

    int ZonesCount { get; }

    public override Zones Generate(Land land)
    {
        if (land.Locations.Count == 0)
        {
            return Zones.Empty;
        }

        Random random = Random.Shared;

        (int, int)[] zoneCenters = GenerateZoneCenters(random, land, ZonesCount).ToArray();
        if (zoneCenters.Length < ZonesCount)
        {
            _logger.LogWarning("Could not generate enough zones: ran out of fuel");
        }

        Dictionary<(int, int), int> zoning = new();

        foreach ((int X, int Y) location in land.Locations)
        {
            zoning[location] = ComputeZone(location, zoneCenters);
        }

        (int, int)[][] zoningArr = zoning.GroupBy(kv => kv.Value).Select(g => g.Select(kv => kv.Key).ToArray()).ToArray();
        return new Zones(zoningArr);
    }

    static IEnumerable<(int, int)> GenerateZoneCenters(Random random, Land land, int count)
    {
        HashSet<(int, int)> zoneCenters = [];
        int fuel = count * 1000;

        while (zoneCenters.Count < count && fuel > 0)
        {
            (int X, int Y) zoneCenter = random.Choose(land.Locations);
            zoneCenters.Add(zoneCenter);

            fuel--;
        }

        return zoneCenters;
    }

    static int ComputeZone((int X, int Y) location, IReadOnlyList<(int, int)> zoneCenters)
    {
        int minDist = int.MaxValue;
        int zone = -1;

        for (int i = 0; i < zoneCenters.Count; i++)
        {
            int dist = ComputeDistance(location, zoneCenters[i]);
            if (dist < minDist)
            {
                minDist = dist;
                zone = i;
            }
        }

        return zone;
    }

    static int ComputeDistance((int X, int Y) p1, (int X, int Y) p2) => Math.Abs(p2.X - p1.X) + Math.Abs(p2.Y - p1.Y);
}
