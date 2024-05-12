using RestAdventure.Core.Combat;

namespace RestAdventure.Core.Entities.Monsters;

public class MonsterCombatEntity : ICombatEntity
{
    public MonsterCombatEntity(MonsterSpecies species, int level)
    {
        Name = species.Name;
        Level = level;
        Health = species.Health;
        MaxHealth = species.Health;
        Speed = species.Speed;
        Attack = species.Attack;
    }

    public CombatEntityId Id { get; } = new(Guid.NewGuid());
    public string Name { get; }
    public int Level { get; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Speed { get; set; }
    public int Attack { get; set; }
    public CombatEntityAttack DealAttack() => new() { Damage = Attack };

    public CombatEntityAttack ReceiveAttack(CombatEntityAttack attack)
    {
        Health -= attack.Damage;
        return attack;
    }
}
