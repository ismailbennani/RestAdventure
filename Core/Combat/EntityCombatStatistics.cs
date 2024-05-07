namespace RestAdventure.Core.Combat;

public class EntityCombatStatistics
{
    public int Health { get; private set; }
    public int Speed { get; private set; }

    public event EventHandler<EntityDamagedEvent>? Damaged;

    public int Attack() => 1;

    public int ReceiveAttack(int value)
    {
        int oldHealth = Health;
        Health -= value;

        Damaged?.Invoke(this, new EntityDamagedEvent { OriginalDamage = value, DamageReceived = value, OldHealth = oldHealth, NewHealth = Health });

        return value;
    }
}

public class EntityDamagedEvent
{
    public required int OriginalDamage { get; init; }
    public required int DamageReceived { get; init; }
    public required int OldHealth { get; init; }
    public required int NewHealth { get; init; }
}
