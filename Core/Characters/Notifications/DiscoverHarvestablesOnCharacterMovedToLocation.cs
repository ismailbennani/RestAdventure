using MediatR;
using RestAdventure.Core.Entities.Notifications;
using RestAdventure.Core.Maps.Harvestables;

namespace RestAdventure.Core.Characters.Notifications;

public class DiscoverHarvestablesOnCharacterMovedToLocation : INotificationHandler<GameEntityMovedToLocation>
{
    readonly GameService _gameService;

    public DiscoverHarvestablesOnCharacterMovedToLocation(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(GameEntityMovedToLocation notification, CancellationToken cancellationToken)
    {
        if (notification.Entity is not Character character)
        {
            return Task.CompletedTask;
        }

        GameContent content = _gameService.RequireGameContent();

        IEnumerable<HarvestableInstance> harvestables = content.Maps.Harvestables.AtLocation(notification.NewLocation);
        foreach (HarvestableInstance harvestableInstance in harvestables)
        {
            character.Player.Knowledge.Discover(harvestableInstance.Harvestable);
        }

        return Task.CompletedTask;
    }
}
