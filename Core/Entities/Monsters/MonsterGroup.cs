using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Pve;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Entities.Monsters;

public record MonsterGroupId(Guid Guid) : GameEntityId(Guid);

/// <summary>
///     Monster groups are the materialization of monsters in the world. Spawning one entity per group help reduce the overall number of entities
/// </summary>
public class MonsterGroup : GameEntity<MonsterGroupId>, IGameEntityWithCombatCapabilities
{
    public MonsterGroup(IReadOnlyList<MonsterInGroup> monsters, Location location) : base(new MonsterGroupId(Guid.NewGuid()), GetName(monsters), location)
    {
        Monsters = monsters;
        StartCombatAction = new StartAndPlayPveCombatAction(this);
    }

    public IReadOnlyList<MonsterInGroup> Monsters { get; }
    public StartAndPlayPveCombatAction StartCombatAction { get; }
    public JoinAndPlayPveCombatAction? JoinCombatAction { get; set; }

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

    public IEnumerable<ICombatEntity> SpawnCombatEntities() => Monsters.Select(m => new MonsterCombatEntity(m.Species, m.Level));
    public void DestroyCombatEntities(IEnumerable<ICombatEntity> entities) { }
}
