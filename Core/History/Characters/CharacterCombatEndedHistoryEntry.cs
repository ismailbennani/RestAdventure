using MediatR;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Combat.Old;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;
using CombatInstance = RestAdventure.Core.Combat.CombatInstance;

namespace RestAdventure.Core.History.Characters;

public class CharacterCombatEndedHistoryEntry : CharacterHistoryEntry
{
    public CharacterCombatEndedHistoryEntry(Character character, CombatInstance combat, long tick) : base(character, tick)
    {
        CombatInstanceId = combat.Id;
        LocationId = combat.Location.Id;
        LocationAreaId = combat.Location.Area.Id;
        LocationAreaName = combat.Location.Area.Name;
        LocationPositionX = combat.Location.PositionX;
        LocationPositionY = combat.Location.PositionY;
        Attackers = combat.Attackers.Entities.Select(e => (e.Id, e.Name)).ToArray();
        Defenders = combat.Defenders.Entities.Select(e => (e.Id, e.Name)).ToArray();
        Winner = combat.Winner ?? throw new ArgumentNullException(nameof(combat.Winner));
        Duration = combat.Turn;
    }

    public CombatInstanceId CombatInstanceId { get; }
    public LocationId LocationId { get; }
    public MapAreaId LocationAreaId { get; }
    public string LocationAreaName { get; }
    public int LocationPositionX { get; }
    public int LocationPositionY { get; }
    public IReadOnlyList<(GameEntityId Id, string Name)> Attackers { get; }
    public IReadOnlyList<(GameEntityId Id, string Name)> Defenders { get; }
    public CombatSide Winner { get; }
    public int Duration { get; }
}

public class CreateCharacterEndedCombatHistoryEntry : INotificationHandler<CombatEnded>
{
    readonly GameService _gameService;

    public CreateCharacterEndedCombatHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(CombatEnded notification, CancellationToken cancellationToken)
    {
        Game state = _gameService.RequireGame();

        foreach (IGameEntityWithCombatCapabilities entity in notification.Combat.Attackers.Entities.Concat(notification.Combat.Defenders.Entities))
        {
            if (entity is not Character character)
            {
                continue;
            }

            CharacterCombatEndedHistoryEntry entry = new(character, notification.Combat, state.Tick);
            state.History.Record(entry);
        }
        return Task.CompletedTask;
    }
}
