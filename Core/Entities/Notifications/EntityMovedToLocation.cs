using MediatR;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Entities.Notifications;

public class EntityMovedToLocation : INotification
{
    public required Entity Entity { get; init; }
    public required Location? OldLocation { get; init; }
    public required Location NewLocation { get; init; }
}
