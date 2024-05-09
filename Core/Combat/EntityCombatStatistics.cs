namespace RestAdventure.Core.Combat;

public class EntityCombatStatistics
{
    public EntityCombatStatistics(int health, int speed, int attack)
    {
        Health = health;
        MaxHealth = health;
        Speed = speed;
        Attack = attack;
    }

    public int Health { get; private set; }
    public int MaxHealth { get; private set; }
    public int Speed { get; private set; }
    public int Attack { get; private set; }

    public event EventHandler<EntityDamagedEvent>? Damaged;

    public EntityAttack DealAttack() => new() { Damage = Attack };

    public EntityAttack ReceiveAttack(EntityAttack attack)
    {
        int oldHealth = Health;
        Health = Math.Max(0, Health - attack.Damage);

        Damaged?.Invoke(this, new EntityDamagedEvent { AttackDealt = attack, AttackReceived = attack, OldHealth = oldHealth, NewHealth = Health });

        return attack;
    }

    public void Revive() => Health = 1;
}

public class EntityDamagedEvent
{
    public required EntityAttack AttackDealt { get; init; }
    public required EntityAttack AttackReceived { get; init; }
    public required int OldHealth { get; init; }
    public required int NewHealth { get; init; }
}
