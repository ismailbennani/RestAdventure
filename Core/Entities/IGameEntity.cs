using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Entities;

public interface IGameEntity : IDisposable
{
    /// <summary>
    ///     The unique ID of the entity
    /// </summary>
    GameEntityId Id { get; }

    /// <summary>
    ///     The name of the entity
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     The location of the entity
    /// </summary>
    Location Location { get; }

    /// <summary>
    ///     Event fired each time this entity moves
    /// </summary>
    event EventHandler<EntityMovedEvent>? Moved;

    /// <summary>
    ///     Kill the entity
    /// </summary>
    Task KillAsync(GameState state);
}
