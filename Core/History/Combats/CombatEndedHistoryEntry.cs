using MediatR;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Notifications;
using CombatInstance = RestAdventure.Core.Combat.CombatInstance;

namespace RestAdventure.Core.History.Combats;

public class CombatEndedHistoryEntry : CombatHistoryEntry
{
    public CombatEndedHistoryEntry(CombatInstance combat, long tick) : base(combat, tick)
    {
        Winner = combat.Winner ?? throw new ArgumentNullException(nameof(combat.Winner));
    }

    public CombatSide Winner { get; }
}

public class CreateCharacterEndedMonsterCombatHistoryEntry : INotificationHandler<CombatEnded>
{
    readonly GameService _gameService;

    public CreateCharacterEndedMonsterCombatHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(CombatEnded notification, CancellationToken cancellationToken)
    {
        Game state = _gameService.RequireGame();
        CombatEndedHistoryEntry entry = new(notification.Combat, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
