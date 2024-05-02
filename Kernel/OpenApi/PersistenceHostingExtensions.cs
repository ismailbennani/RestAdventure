using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestAdventure.Kernel.Persistence;
using Xtensive.Orm;
using Xtensive.Orm.Configuration;

namespace RestAdventure.Kernel.OpenApi;

public static class PersistenceHostingExtensions
{
    public static async Task SetupPersistence(this IHostApplicationBuilder builder, ILogger logger, params Assembly[] assembliesToLoad)
    {
        string connectionString = builder.Configuration.GetConnectionString("Main") ?? throw new InvalidOperationException("Missing connection string");
        DomainUpgradeMode? upgradeMode = ParseUpgradeMode(builder.Configuration["Persistence:UpgradeMode"]);

        if (!upgradeMode.HasValue)
        {
            throw new InvalidOperationException($"Invalid upgrade mode: {upgradeMode}");
        }

        DomainConfiguration? config = new(connectionString)
        {
            UpgradeMode = upgradeMode.Value
        };

        config.Types.Register(typeof(PersistenceHostingExtensions).Assembly);

        foreach (Assembly assembly in assembliesToLoad)
        {
            config.Types.Register(assembly);
        }

        logger.LogInformation("Building domain in mode: {mode}...", upgradeMode);

        Domain domain = await Domain.BuildAsync(config);

        logger.LogInformation("Domain built successfully.");

        builder.Services.AddSingleton(new DomainAccessor(domain));
    }

    static DomainUpgradeMode? ParseUpgradeMode(string? str)
    {
        if (str == null)
        {
            return DomainUpgradeMode.Default;
        }

        Dictionary<string, DomainUpgradeMode> mapping = new()
        {
            { "Skip", DomainUpgradeMode.Skip },
            { "Validate", DomainUpgradeMode.Validate },
            { "Recreate", DomainUpgradeMode.Recreate },
            { "Perform", DomainUpgradeMode.Perform },
            { "PerformSafely", DomainUpgradeMode.PerformSafely },
            { "LegacySkip", DomainUpgradeMode.LegacySkip },
            { "LegacyValidate", DomainUpgradeMode.LegacyValidate },
            { "Default", DomainUpgradeMode.Default }
        };

        foreach (KeyValuePair<string, DomainUpgradeMode> entry in mapping)
        {
            if (str.Equals(entry.Key, StringComparison.InvariantCultureIgnoreCase))
            {
                return entry.Value;
            }
        }

        return null;
    }
}
