using MediatR;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.History.Characters;

public class CharacterCombatStartedHistoryEntry : CharacterHistoryEntry
{
    public CharacterCombatStartedHistoryEntry(Character character, CombatInstance combat, long tick) : base(character, tick)
    {
        CombatInstanceId = combat.Id;
        LocationId = combat.Location.Id;
        LocationAreaId = combat.Location.Area.Id;
        LocationAreaName = combat.Location.Area.Name;
        LocationPositionX = combat.Location.PositionX;
        LocationPositionY = combat.Location.PositionY;
        Team1 = combat.Team1.Entities.Select(e => (e.Id, e.Name)).ToArray();
        Team2 = combat.Team2.Entities.Select(e => (e.Id, e.Name)).ToArray();
    }

    public CombatInstanceId CombatInstanceId { get; }
    public LocationId LocationId { get; }
    public MapAreaId LocationAreaId { get; }
    public string LocationAreaName { get; }
    public int LocationPositionX { get; }
    public int LocationPositionY { get; }
    public IReadOnlyList<(GameEntityId Id, string Name)> Team1 { get; }
    public IReadOnlyList<(GameEntityId Id, string Name)> Team2 { get; }
}

public class CreateCharacterStartedCombatHistoryEntry : INotificationHandler<CombatStarted>
{
    readonly GameService _gameService;

    public CreateCharacterStartedCombatHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(CombatStarted notification, CancellationToken cancellationToken)
    {
        GameState state = _gameService.RequireGameState();

        foreach (IGameEntityWithCombatStatistics entity in notification.Combat.Team1.Entities.Concat(notification.Combat.Team2.Entities))
        {
            if (entity is not Character character)
            {
                continue;
            }

            CharacterCombatStartedHistoryEntry entry = new(character, notification.Combat, state.Tick);
            state.History.Record(entry);
        }
        return Task.CompletedTask;
    }
}
