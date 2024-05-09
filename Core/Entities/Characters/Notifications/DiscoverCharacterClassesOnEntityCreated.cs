using MediatR;
using RestAdventure.Core.Content;
using RestAdventure.Core.Entities.Notifications;

namespace RestAdventure.Core.Entities.Characters.Notifications;

public class DiscoverCharacterClassesOnEntityCreated : INotificationHandler<GameEntityCreated>
{
    readonly GameService _gameService;

    public DiscoverCharacterClassesOnEntityCreated(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(GameEntityCreated notification, CancellationToken cancellationToken)
    {
        if (notification.Entity is not Character character)
        {
            return Task.CompletedTask;
        }

        GameContent content = _gameService.RequireGameContent();

        foreach (CharacterClass cls in content.Characters.Classes)
        {
            character.Player.Knowledge.Discover(cls);
        }

        return Task.CompletedTask;
    }
}
