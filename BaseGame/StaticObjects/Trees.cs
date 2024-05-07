using System.Reflection;
using RestAdventure.Core.StaticObjects;

namespace BaseGame.StaticObjects;

public class Trees
{
    public Trees()
    {
        AppleTree = new StaticObject { Name = "Apple Tree" };
        PearTree = new StaticObject { Name = "Pear Tree" };
    }

    public StaticObject AppleTree { get; set; }
    public StaticObject PearTree { get; set; }

    public IEnumerable<StaticObject> All =>
        typeof(Trees).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.PropertyType == typeof(StaticObject))
            .Select(p => p.GetValue(this))
            .OfType<StaticObject>();
}
