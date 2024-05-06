﻿using MediatR;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Notifications;

namespace RestAdventure.Core.History.Entities;

public class EntityDeletedHistoryEntry : EntityHistoryEntry
{
    public EntityDeletedHistoryEntry(IGameEntity entity, long tick) : base(entity, tick)
    {
    }
}

public class CreateEntityDeletedHistoryEntry : INotificationHandler<GameEntityDeleted>
{
    readonly GameService _gameService;

    public CreateEntityDeletedHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(GameEntityDeleted notification, CancellationToken cancellationToken)
    {
        GameState state = _gameService.RequireGameState();
        EntityDeletedHistoryEntry entry = new(notification.Entity, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}