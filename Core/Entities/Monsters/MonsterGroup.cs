using RestAdventure.Core.Combat.Pve;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Entities.Monsters;

public record MonsterGroupId(Guid Guid) : GameEntityId(Guid);

/// <summary>
///     Monster groups are the materialization of monsters in the world. Spawning one entity per group help reduce the overall number of entities
/// </summary>
public class MonsterGroup : GameEntity<MonsterGroupId>
{
    public MonsterGroup(IReadOnlyList<MonsterInGroup> monsters, Location location) : base(new MonsterGroupId(Guid.NewGuid()), GetName(monsters), location)
    {
        Monsters = monsters;
        CombatAction = new PveCombatAction(this);
    }

    public IReadOnlyList<MonsterInGroup> Monsters { get; }
    public PveCombatAction CombatAction { get; }

    static string GetName(IEnumerable<MonsterInGroup> monsters) =>
        string.Join(
            ", ",
            monsters.GroupBy(m => m.Species)
                .Select(
                    g =>
                    {
                        int count = g.Count();
                        return count == 1 ? g.Key.Name : $"{count}x {g.Key.Name}";
                    }
                )
        );
}
