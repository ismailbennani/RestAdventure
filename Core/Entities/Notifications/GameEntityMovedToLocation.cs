using MediatR;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Entities.Notifications;

public class GameEntityMovedToLocation : INotification
{
    public required GameEntity Entity { get; init; }
    public required Location? OldLocation { get; init; }
    public required Location NewLocation { get; init; }
}
