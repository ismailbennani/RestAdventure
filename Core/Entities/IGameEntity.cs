using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Entities;

public interface IGameEntity
{
    /// <summary>
    ///     The unique ID of the entity
    /// </summary>
    GameEntityId Id { get; }

    /// <summary>
    ///     The team of the character
    /// </summary>
    ITeam? Team { get; }

    /// <summary>
    ///     The name of the entity
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     The location of the entity
    /// </summary>
    Location Location { get; }

    /// <summary>
    ///     If the character is busy, they cannot perform or be the target of actions
    /// </summary>
    bool Busy { get; }
}
