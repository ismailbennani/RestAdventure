﻿using MediatR;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Notifications;

namespace RestAdventure.Core.History.Combats;

public class CombatStartedHistoryEntry : CombatHistoryEntry
{
    public CombatStartedHistoryEntry(CombatInstance combat, long tick) : base(combat, tick)
    {
    }
}

public class CreateCharacterStartedMonsterCombatHistoryEntry : INotificationHandler<CombatStarted>
{
    readonly GameService _gameService;

    public CreateCharacterStartedMonsterCombatHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(CombatStarted notification, CancellationToken cancellationToken)
    {
        Game state = _gameService.RequireGame();
        CombatStartedHistoryEntry entry = new(notification.Combat, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
