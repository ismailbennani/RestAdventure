using MediatR;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Entities;

namespace RestAdventure.Core.History.Characters;

public class CharacterReceivedAttackHistoryEntry : CharacterHistoryEntry
{
    public CharacterReceivedAttackHistoryEntry(Character source, EntityAttack attackDealt, EntityAttack attackReceived, IGameEntityWithCombatStatistics attacker, long tick) : base(
        source,
        tick
    )
    {
        AttackDealt = attackDealt;
        AttackReceived = attackReceived;
        AttackerId = attacker.Id;
        AttackerName = attacker.Name;
    }

    public EntityAttack AttackDealt { get; }
    public EntityAttack AttackReceived { get; }
    public GameEntityId AttackerId { get; }
    public string AttackerName { get; }
}

public class CreateCharacterReceivedAttackHistoryEntry : INotificationHandler<CombatEntityAttacked>
{
    readonly GameService _gameService;

    public CreateCharacterReceivedAttackHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(CombatEntityAttacked notification, CancellationToken cancellationToken)
    {
        if (notification.Target is not Character character)
        {
            return Task.CompletedTask;
        }

        GameState state = _gameService.RequireGameState();
        CharacterReceivedAttackHistoryEntry entry = new(character, notification.AttackDealt, notification.AttackReceived, notification.Attacker, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
