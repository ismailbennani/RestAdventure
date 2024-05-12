using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities.Monsters;

namespace RestAdventure.Core.Serialization.Entities;

public class MonsterGroupSnapshot : GameEntitySnapshot<MonsterGroupId>
{
    public MonsterGroupSnapshot(MonsterGroupId id) : base(id)
    {
    }

    public required IReadOnlyList<MonsterInGroup> Monsters { get; init; }
    public required CombatInstanceId? OngoingCombatId { get; init; }

    public static MonsterGroupSnapshot Take(MonsterGroup group) =>
        new(group.Id)
        {
            Team = group.Team == null ? null : TeamSnapshot.Take(group.Team),
            Name = group.Name,
            Location = group.Location,
            Busy = group.Busy,
            Monsters = group.Monsters.ToList(),
            OngoingCombatId = group.OngoingCombat?.Id
        };
}
