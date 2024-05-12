using RestAdventure.Core.Combat;

namespace RestAdventure.Core.Entities.Characters.Combats;

public class CharacterCombatEntity : ICombatEntity
{
    readonly Character _character;

    public CharacterCombatEntity(Character character)
    {
        _character = character;
        MaxHealth = character.Class.Health;
        Speed = character.Class.Speed;
        Attack = character.Class.Attack;
    }

    public CombatEntityId Id { get; } = new(Guid.NewGuid());
    public string Name => _character.Name;
    public int Level => _character.Level;
    public int Health { get => _character.Health; set => _character.Health = value; }
    public int MaxHealth { get; set; }
    public int Speed { get; set; }
    public int Attack { get; set; }

    public CombatEntityAttack DealAttack() =>
        new()
        {
            Damage = Attack
        };

    public CombatEntityAttack ReceiveAttack(CombatEntityAttack attack)
    {
        Health -= attack.Damage;
        return attack;
    }
}
