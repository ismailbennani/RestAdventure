using MediatR;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Characters;

namespace RestAdventure.Core.History.Characters;

public class CharacterAttackedHistoryEntry : CharacterHistoryEntry
{
    public CharacterAttackedHistoryEntry(Character character, EntityAttack attackDealt, EntityAttack attackReceived, IGameEntityWithCombatStatistics target, long tick) : base(
        character,
        tick
    )
    {
        AttackDealt = attackDealt;
        AttackReceived = attackReceived;
        TargetId = target.Id;
        TargetName = target.Name;
    }

    public EntityAttack AttackDealt { get; }
    public EntityAttack AttackReceived { get; }
    public GameEntityId TargetId { get; }
    public string TargetName { get; }
}

public class CreateCharacterAttackedHistoryEntry : INotificationHandler<CombatEntityAttacked>
{
    readonly GameService _gameService;

    public CreateCharacterAttackedHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(CombatEntityAttacked notification, CancellationToken cancellationToken)
    {
        if (notification.Attacker is not Character character)
        {
            return Task.CompletedTask;
        }

        GameState state = _gameService.RequireGameState();
        CharacterAttackedHistoryEntry entry = new(character, notification.AttackDealt, notification.AttackReceived, notification.Target, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
