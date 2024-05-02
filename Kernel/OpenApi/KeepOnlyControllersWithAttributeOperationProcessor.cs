using System.Reflection;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace RestAdventure.Kernel.OpenApi;

public class KeepOnlyControllersWithAttributeOperationProcessor : IOperationProcessor
{
    readonly Type[] _attributes;

    public KeepOnlyControllersWithAttributeOperationProcessor(params Type[] attributes)
    {
        _attributes = attributes;
    }

    public AggregationMode Aggregation { get; set; } = AggregationMode.Any;
    public bool Invert { get; set; } = false;

    public bool Process(OperationProcessorContext context)
    {
        IEnumerable<bool> hasAttributes = _attributes.Select(attribute => context.ControllerType.GetCustomAttribute(attribute) != null);

        bool result;
        switch (Aggregation)
        {
            case AggregationMode.Any:
                result = hasAttributes.Any(b => b);
                break;
            case AggregationMode.All:
                result = hasAttributes.All(b => b);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(Aggregation), Aggregation, null);
        }

        return Invert ? !result : result;
    }

    public enum AggregationMode
    {
        Any,
        All
    }
}
