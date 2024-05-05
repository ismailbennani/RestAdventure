using Microsoft.Extensions.DependencyInjection;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Characters.Services;
using RestAdventure.Core.Gameplay.Actions;

namespace RestAdventure.Core;

public static class CoreHostingExtensions
{
    public static void ConfigureCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<GameService>();
        services.AddSingleton<CharactersService>();
        services.AddSingleton<CharacterActionsService>();
    }
}
