using System.Reflection;

namespace SandboxGame;

public static class ObjectExplorer
{
    public static IEnumerable<TValue> FindValuesOfType<T, TValue>(T instance) => FindSingleValuesOfType<T, TValue>(instance).Concat(FindEnumerablesOfType<T, TValue>(instance));

    static IEnumerable<TValue> FindSingleValuesOfType<T, TValue>(T instance)
    {
        Type valueType = typeof(TValue);
        return instance?.GetType()
                   .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                   .Where(p => p.PropertyType.IsAssignableTo(valueType))
                   .Select(p => p.GetValue(instance))
                   .OfType<TValue>()
               ?? Enumerable.Empty<TValue>();
    }

    static IEnumerable<TValue> FindEnumerablesOfType<T, TValue>(T instance)
    {
        Type enumerableType = typeof(IEnumerable<TValue>);
        return instance?.GetType()
                   .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                   .Where(p => p.PropertyType.IsAssignableTo(enumerableType))
                   .Select(p => p.GetValue(instance))
                   .OfType<IEnumerable<TValue>>()
                   .SelectMany(e => e)
               ?? Enumerable.Empty<TValue>();
    }
}
