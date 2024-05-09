﻿using MediatR;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Notifications;

namespace RestAdventure.Core.History.Combats;

public class CombatPreparationStartedHistoryEntry : CombatHistoryEntry
{
    public CombatPreparationStartedHistoryEntry(CombatInPreparation combat, long tick) : base(combat, tick)
    {
    }
}

public class CreateCharacterPreparationStartedMonsterCombatHistoryEntry : INotificationHandler<CombatPreparationStarted>
{
    readonly GameService _gameService;

    public CreateCharacterPreparationStartedMonsterCombatHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(CombatPreparationStarted notification, CancellationToken cancellationToken)
    {
        GameState state = _gameService.RequireGameState();
        CombatPreparationStartedHistoryEntry entry = new(notification.Combat, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}