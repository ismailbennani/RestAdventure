using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Kernel.OpenApi;

public class SetServerBasePathDocumentProcessor : IDocumentProcessor
{
    readonly string _basePath;

    public SetServerBasePathDocumentProcessor(string basePath)
    {
        _basePath = basePath;
    }

    public void Process(DocumentProcessorContext context)
    {
        foreach (OpenApiServer server in context.Document.Servers)
        {
            Uri uri = new(new Uri(server.Url), _basePath);
            server.Url += uri.AbsolutePath;
        }
    }
}
