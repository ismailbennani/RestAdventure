using RestAdventure.Core.Settings;

namespace RestAdventure.Core.Combat;

public class CombatFormationInPreparation
{
    readonly GameSettings _gameSettings;
    List<IGameEntityWithCombatStatistics> _entities;

    public CombatFormationInPreparation(IGameEntityWithCombatStatistics entity, GameSettings gameSettings)
    {
        _gameSettings = gameSettings;
        Owner = entity;
        _entities = [entity];
        CombatEntityKind = entity.CombatEntityKind;
    }

    public IGameEntityWithCombatStatistics Owner { get; }

    /// <summary>
    ///     The characters in order
    /// </summary>
    public IReadOnlyList<IGameEntityWithCombatStatistics> Entities => _entities;

    public CombatEntityKind CombatEntityKind { get; }

    public int MaxCount =>
        CombatEntityKind switch
        {
            CombatEntityKind.Character => _gameSettings.Combat.MaxCharacterTeamSize,
            CombatEntityKind.Monster => _gameSettings.Combat.MaxMonsterTeamSize,
            _ => throw new ArgumentOutOfRangeException()
        };

    public void Add(IGameEntityWithCombatStatistics entity)
    {
        if (entity.CombatEntityKind != CombatEntityKind)
        {
            throw new InvalidOperationException($"Cannot add entity of kind {entity.CombatEntityKind}");
        }

        _entities.Add(entity);
    }

    public void Remove(IGameEntityWithCombatStatistics entity)
    {
        if (entity == Owner)
        {
            throw new InvalidOperationException("Cannot remove the owner of the formation");
        }

        if (!_entities.Remove(entity))
        {
            throw new InvalidOperationException("Cannot remove entity because it is not part of the formation");
        }
    }

    public void Reorder(IReadOnlyList<IGameEntityWithCombatStatistics> newOrder)
    {
        IEnumerable<IGameEntityWithCombatStatistics> intersection = newOrder.Intersect(Entities);
        int intersectionCount = intersection.Count();
        if (intersectionCount != newOrder.Count || intersectionCount != Entities.Count)
        {
            throw new InvalidOperationException("Bad new order");
        }

        _entities = newOrder.ToList();
    }

    public CombatFormation Lock() => new() { Entities = Entities };
}
