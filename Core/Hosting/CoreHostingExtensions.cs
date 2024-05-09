using Microsoft.Extensions.DependencyInjection;
using RestAdventure.Core.Characters.Services;

namespace RestAdventure.Core.Hosting;

public static class CoreHostingExtensions
{
    public static void ConfigureCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<GameService>();
        services.AddSingleton<CharactersService>();
    }
}
