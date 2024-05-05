using MediatR;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Characters.Notifications;

public class CharacterMovedToLocation : INotification
{
    public required Character Character { get; init; }
    public required MapLocation Location { get; init; }
}
