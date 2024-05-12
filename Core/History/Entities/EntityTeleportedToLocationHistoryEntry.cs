using MediatR;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Notifications;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.History.Entities;

public class EntityTeleportedToLocationHistoryEntry : EntityHistoryEntry
{
    public EntityTeleportedToLocationHistoryEntry(IGameEntity entity, Location? oldLocation, Location newLocation, long tick) : base(entity, tick)
    {
        OldLocationId = oldLocation?.Id;
        OldLocationPositionX = oldLocation?.PositionX;
        OldLocationPositionY = oldLocation?.PositionY;
        OldLocationAreaId = oldLocation?.Area.Id;
        OldLocationAreaName = oldLocation?.Area.Name;

        NewLocationId = newLocation.Id;
        NewLocationPositionX = newLocation.PositionX;
        NewLocationPositionY = newLocation.PositionY;
        NewLocationAreaId = newLocation.Area.Id;
        NewLocationAreaName = newLocation.Area.Name;
    }

    public LocationId? OldLocationId { get; }
    public int? OldLocationPositionX { get; }
    public int? OldLocationPositionY { get; }
    public MapAreaId? OldLocationAreaId { get; }
    public string? OldLocationAreaName { get; }

    public LocationId NewLocationId { get; }
    public int NewLocationPositionX { get; }
    public int NewLocationPositionY { get; }
    public MapAreaId NewLocationAreaId { get; }
    public string NewLocationAreaName { get; }
}

public class CreateEntityTeleportedToLocationHistoryEntry : INotificationHandler<GameEntityTeleportedToLocation>
{
    readonly GameService _gameService;

    public CreateEntityTeleportedToLocationHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(GameEntityTeleportedToLocation notification, CancellationToken cancellationToken)
    {
        Game state = _gameService.RequireGameState();
        EntityTeleportedToLocationHistoryEntry entry = new(notification.Entity, notification.OldLocation, notification.NewLocation, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
