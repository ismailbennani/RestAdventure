using Microsoft.Extensions.DependencyInjection;
using RestAdventure.Core.Characters;

namespace RestAdventure.Core;

public static class CoreHostingExtensions
{
    public static void ConfigureCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<GameService>();
        services.AddSingleton<CharactersInteractionService>();
    }
}
