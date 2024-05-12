namespace RestAdventure.Core.Utils;

public class Weighted<T>
{
    public Weighted(T value, double weight)
    {
        Value = value;
        Weight = weight;
    }

    public T Value { get; }
    public double Weight { get; }
}
