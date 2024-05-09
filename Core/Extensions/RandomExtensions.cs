namespace RestAdventure.Core.Extensions;

public static class RandomExtensions
{
    public static T Choose<T>(this Random random, IReadOnlyCollection<T> elements) => elements.ElementAt(random.Next(0, elements.Count));
}
