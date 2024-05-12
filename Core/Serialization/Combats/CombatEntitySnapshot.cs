using RestAdventure.Core.Combat;

namespace RestAdventure.Core.Serialization.Combats;

public class CombatEntitySnapshot
{
    public CombatEntitySnapshot(CombatEntityId id)
    {
        Id = id;
    }

    public CombatEntityId Id { get; }
    public required string Name { get; init; }
    public required int Level { get; init; }
    public required int Health { get; init; }
    public required int MaxHealth { get; init; }

    public static CombatEntitySnapshot Take(ICombatEntity entity) =>
        new(entity.Id)
        {
            Name = entity.Name,
            Level = entity.Level,
            Health = entity.Health,
            MaxHealth = entity.MaxHealth
        };
}
