using RestAdventure.Core.Combat.Old;

namespace RestAdventure.Core.Combat;

public record CombatEntityId(Guid Guid);

public interface ICombatEntity
{
    CombatEntityId Id { get; }
    string Name { get; }
    int Level { get; }
    int Health { get; set; }
    int MaxHealth { get; set; }
    int Speed { get; set; }
    int Attack { get; set; }

    CombatEntityAttack DealAttack();
    CombatEntityAttack ReceiveAttack(CombatEntityAttack attack);
}
