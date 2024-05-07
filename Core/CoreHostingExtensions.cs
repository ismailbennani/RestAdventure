using Microsoft.Extensions.DependencyInjection;
using RestAdventure.Core.Characters.Services;
using RestAdventure.Core.Interactions.Providers;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Monsters;

namespace RestAdventure.Core;

public static class CoreHostingExtensions
{
    public static void ConfigureCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<GameService>();
        services.AddSingleton<CharactersService>();

        // IInteractionProvider
        services.AddSingleton<IInteractionProvider, JobInteractionProvider>();
        services.AddSingleton<IInteractionProvider, MonsterCombatInteractionProvider>();
    }
}
