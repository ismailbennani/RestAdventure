using System.Reflection;

namespace ExampleGame;

public static class ObjectExplorer
{
    public static IEnumerable<TValue> FindValuesOfType<T, TValue>(T instance)
    {
        Type valueType = typeof(TValue);
        return instance?.GetType()
                   .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                   .Where(p => p.PropertyType == valueType)
                   .Select(p => p.GetValue(instance))
                   .OfType<TValue>()
               ?? Enumerable.Empty<TValue>();
    }
}
