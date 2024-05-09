﻿using MediatR;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.History.Characters;

public class CharacterStartedCombatPreparationHistoryEntry : CharacterHistoryEntry
{
    public CharacterStartedCombatPreparationHistoryEntry(Character character, CombatInPreparation combat, long tick) : base(character, tick)
    {
        CombatInstanceId = combat.Id;
        LocationId = combat.Location.Id;
        LocationAreaId = combat.Location.Area.Id;
        LocationAreaName = combat.Location.Area.Name;
        LocationPositionX = combat.Location.PositionX;
        LocationPositionY = combat.Location.PositionY;
        Attackers = combat.Attackers.Entities.Select(e => (e.Id, e.Name)).ToArray();
        Defenders = combat.Defenders.Entities.Select(e => (e.Id, e.Name)).ToArray();
    }

    public CombatInstanceId CombatInstanceId { get; }
    public LocationId LocationId { get; }
    public MapAreaId LocationAreaId { get; }
    public string LocationAreaName { get; }
    public int LocationPositionX { get; }
    public int LocationPositionY { get; }
    public IReadOnlyList<(GameEntityId Id, string Name)> Attackers { get; }
    public IReadOnlyList<(GameEntityId Id, string Name)> Defenders { get; }
}

public class CreateCharacterStartedCombatPreparationHistoryEntry : INotificationHandler<CombatPreparationStarted>
{
    readonly GameService _gameService;

    public CreateCharacterStartedCombatPreparationHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(CombatPreparationStarted notification, CancellationToken cancellationToken)
    {
        GameState state = _gameService.RequireGameState();

        foreach (IGameEntityWithCombatStatistics entity in notification.Combat.Attackers.Entities.Concat(notification.Combat.Defenders.Entities))
        {
            if (entity is not Character character)
            {
                continue;
            }

            CharacterStartedCombatPreparationHistoryEntry entry = new(character, notification.Combat, state.Tick);
            state.History.Record(entry);
        }
        return Task.CompletedTask;
    }
}
