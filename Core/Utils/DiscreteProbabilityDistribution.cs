namespace RestAdventure.Core.Utils;

public abstract class ProbabilityDistribution<T>
{
    public abstract T Sample(double random);
}

public class DiscreteProbabilityDistribution<T> : ProbabilityDistribution<T>
{
    readonly IReadOnlyCollection<(double CumulativeWeight, T value)> _cumulativeWeightValues;

    public DiscreteProbabilityDistribution(params Weighted<T>[] weightedValues) : this((IReadOnlyCollection<Weighted<T>>)weightedValues) { }

    public DiscreteProbabilityDistribution(IReadOnlyCollection<Weighted<T>> weightedValues)
    {
        if (weightedValues.Count == 0)
        {
            throw new ArgumentException("No value was provided");
        }

        List<(double, T)> cumulativeWeightValues = new();
        double totalSum = weightedValues.Sum(v => v.Weight);
        double sum = 0.0;
        foreach (Weighted<T> value in weightedValues.OrderBy(w => w.Weight).ToArray())
        {
            sum += value.Weight;
            cumulativeWeightValues.Add((sum / totalSum, value.Value));
        }
        _cumulativeWeightValues = cumulativeWeightValues;
    }

    public override T Sample(double random)
    {
        foreach ((double cumulativeWeight, T value) in _cumulativeWeightValues)
        {
            if (cumulativeWeight > random)
            {
                return value;
            }
        }

        return _cumulativeWeightValues.Last().value;
    }
}
