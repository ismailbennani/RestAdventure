using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Resources;
using RestAdventure.Core.Spawners;

namespace RestAdventure.Core.Maps;

public class GameMaps
{
    public GameResourcesStore<MapAreaId, MapArea> Areas { get; } = new();
    public GameLocations Locations { get; } = new();

    /// <summary>
    ///     The spawners in the game
    /// </summary>
    public GameResourcesStore<SpawnerId, Spawner> Spawners { get; } = new();
}
