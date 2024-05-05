using MediatR;
using RestAdventure.Core.Maps.Harvestables;

namespace RestAdventure.Core.Characters.Notifications;

public class DiscoverHarvestablesOnCharacterMovedToLocation : INotificationHandler<CharacterMovedToLocation>
{
    readonly GameService _gameService;

    public DiscoverHarvestablesOnCharacterMovedToLocation(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(CharacterMovedToLocation notification, CancellationToken cancellationToken)
    {
        GameContent content = _gameService.RequireGameContent();

        IEnumerable<HarvestableInstance> harvestables = content.Maps.Harvestables.AtLocation(notification.Location);
        foreach (HarvestableInstance harvestableInstance in harvestables)
        {
            notification.Character.Team.Player.Knowledge.Discover(harvestableInstance.Harvestable);
        }

        return Task.CompletedTask;
    }
}
