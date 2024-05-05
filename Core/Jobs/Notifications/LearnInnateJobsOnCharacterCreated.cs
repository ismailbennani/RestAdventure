using MediatR;
using RestAdventure.Core.Characters.Notifications;

namespace RestAdventure.Core.Jobs.Notifications;

public class LearnInnateJobsOnCharacterCreated : INotificationHandler<CharacterCreated>
{
    readonly GameService _gameService;

    public LearnInnateJobsOnCharacterCreated(GameService gameService)
    {
        _gameService = gameService;
    }

    public async Task Handle(CharacterCreated notification, CancellationToken cancellationToken)
    {
        GameContent content = _gameService.RequireGameContent();

        foreach (Job job in content.Jobs.Innate)
        {
            await notification.Character.Jobs.LearnAsync(job);
        }
    }
}
