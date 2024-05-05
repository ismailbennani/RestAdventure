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

        GameState state = _gameService.RequireGameState();

        IEnumerable<HarvestableInstance> harvestables = state.Entities.AtLocation<HarvestableInstance>(notification.NewLocation);
        foreach (HarvestableInstance harvestableInstance in harvestables)
        {
            character.Player.Knowledge.Discover(harvestableInstance.Harvestable);
        }

        return Task.CompletedTask;
    }
}
