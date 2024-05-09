using MediatR;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Entities.Notifications;

public class GameEntityTeleportedToLocation : INotification
{
    public required IGameEntity Entity { get; init; }
    public required Location? OldLocation { get; init; }
    public required Location NewLocation { get; init; }

    public override string ToString() => $"{Entity}[{OldLocation}, {NewLocation}";
}
