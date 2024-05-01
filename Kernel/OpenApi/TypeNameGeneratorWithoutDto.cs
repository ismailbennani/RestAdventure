using NJsonSchema;

namespace Kernel.OpenApi;

public class TypeNameWithoutDtoGenerator : ITypeNameGenerator
{
    readonly ITypeNameGenerator _generator;

    public TypeNameWithoutDtoGenerator(ITypeNameGenerator generator)
    {
        _generator = generator;
    }

    public string Generate(JsonSchema schema, string? typeNameHint, IEnumerable<string> reservedTypeNames)
    {
        string name = _generator.Generate(schema, typeNameHint, reservedTypeNames);
        return name.Replace("dto", "", StringComparison.InvariantCultureIgnoreCase);
    }
}
