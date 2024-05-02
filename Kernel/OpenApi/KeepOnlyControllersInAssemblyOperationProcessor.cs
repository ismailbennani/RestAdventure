using System.Reflection;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace RestAdventure.Kernel.OpenApi;

public class KeepOnlyControllersInAssemblyOperationProcessor : IOperationProcessor
{
    readonly Assembly _assembly;

    public KeepOnlyControllersInAssemblyOperationProcessor(Assembly assembly)
    {
        _assembly = assembly;
    }

    public bool Process(OperationProcessorContext context) => context.ControllerType.Assembly == _assembly;
}
