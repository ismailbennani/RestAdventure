using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Utils;

namespace ContentToolbox.Spawners.EntitySpawners;

public class RandomByAreaLevelStaticObjectSpawner : EntitySpawner
{
    readonly Dictionary<int, IReadOnlyCollection<Weighted<StaticObject>>> _weightsByLevel;
    readonly Dictionary<int, ProbabilityDistribution<StaticObject>> _distributionsByLevel = new();

    public RandomByAreaLevelStaticObjectSpawner(Dictionary<int, IReadOnlyCollection<Weighted<StaticObject>>> weightsByLevel)
    {
        if (weightsByLevel.Count == 0)
        {
            throw new ArgumentException("No weights provided");
        }

        _weightsByLevel = weightsByLevel;
    }

    public override IEnumerable<GameEntity> Spawn(Location location)
    {
        ProbabilityDistribution<StaticObject>? distribution = GetDistribution(location.Area.Level);
        if (distribution == null)
        {
            return Enumerable.Empty<GameEntity>();
        }

        double randomValue = Random.Shared.NextDouble();
        StaticObject staticObject = distribution.Sample(randomValue);
        return [new StaticObjectInstance(staticObject, location)];
    }

    ProbabilityDistribution<StaticObject>? GetDistribution(int level)
    {
        if (_distributionsByLevel.TryGetValue(level, out ProbabilityDistribution<StaticObject>? distribution))
        {
            return distribution;
        }

        IReadOnlyCollection<Weighted<StaticObject>>? weights = GetWeights(level);
        if (weights == null || weights.Count == 0)
        {
            return null;
        }

        DiscreteProbabilityDistribution<StaticObject> newDistribution = new(weights);
        _distributionsByLevel[level] = newDistribution;
        return newDistribution;
    }

    IReadOnlyCollection<Weighted<StaticObject>>? GetWeights(int level)
    {
        if (_weightsByLevel.TryGetValue(level, out IReadOnlyCollection<Weighted<StaticObject>>? weights))
        {
            return weights;
        }

        if (_weightsByLevel.Count == 1)
        {
            return _weightsByLevel.First().Value;
        }

        int? levelBelow = null;
        int? levelAbove = null;
        foreach (KeyValuePair<int, IReadOnlyCollection<Weighted<StaticObject>>> values in _weightsByLevel)
        {
            if (values.Key < level)
            {
                levelBelow = values.Key;
            }
            else
            {
                levelAbove = values.Key;
                break;
            }
        }

        if (!levelBelow.HasValue && !levelAbove.HasValue)
        {
            throw new InvalidOperationException("SHOULD NOT HAPPEN");
        }

        if (!levelBelow.HasValue && levelAbove.HasValue)
        {
            return _weightsByLevel[levelAbove.Value];
        }

        if (!levelAbove.HasValue && levelBelow.HasValue)
        {
            return _weightsByLevel[levelBelow.Value];
        }

        IReadOnlyCollection<Weighted<StaticObject>> weightsBelow = _weightsByLevel[levelBelow!.Value];
        IReadOnlyCollection<Weighted<StaticObject>> weightsAbove = _weightsByLevel[levelAbove!.Value];
        IEnumerable<StaticObject> keys = weightsBelow.Concat(weightsAbove).Select(w => w.Value).Distinct();

        List<Weighted<StaticObject>> result = [];
        foreach (StaticObject staticObject in keys)
        {
            double weightBelow = weightsBelow.Where(w => w.Value == staticObject).Sum(w => w.Weight);
            double weightAbove = weightsAbove.Where(w => w.Value == staticObject).Sum(w => w.Weight);
            double weight = Interpolate.LinearUnclamped(levelBelow.Value, weightBelow, levelAbove.Value, weightAbove, level);
            result.Add(new Weighted<StaticObject>(staticObject, weight));
        }

        return result;
    }
}
