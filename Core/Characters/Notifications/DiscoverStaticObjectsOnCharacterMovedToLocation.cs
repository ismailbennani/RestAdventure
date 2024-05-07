using MediatR;
using RestAdventure.Core.Entities.Notifications;
using RestAdventure.Core.StaticObjects;

namespace RestAdventure.Core.Characters.Notifications;

public class DiscoverStaticObjectsOnCharacterMovedToLocation : INotificationHandler<GameEntityMovedToLocation>
{
    readonly GameService _gameService;

    public DiscoverStaticObjectsOnCharacterMovedToLocation(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(GameEntityMovedToLocation notification, CancellationToken cancellationToken)
    {
        if (notification.Entity is not Character character)
        {
            return Task.CompletedTask;
        }

        GameState state = _gameService.RequireGameState();

        IEnumerable<StaticObjectInstance> staticObjects = state.Entities.AtLocation<StaticObjectInstance>(notification.NewLocation);
        foreach (StaticObjectInstance staticObject in staticObjects)
        {
            character.Player.Knowledge.Discover(staticObject.Object);
        }

        return Task.CompletedTask;
    }
}
