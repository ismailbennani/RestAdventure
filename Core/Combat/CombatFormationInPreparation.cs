﻿using RestAdventure.Core.Settings;

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

    public event EventHandler<IGameEntityWithCombatStatistics>? Added;
    public event EventHandler<IGameEntityWithCombatStatistics>? Removed;
    public event EventHandler<CombatFormationInPreparationReorderedEvent>? Reordered;

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

        Added?.Invoke(this, entity);
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

        Removed?.Invoke(this, entity);
    }

    public void Reorder(IReadOnlyList<IGameEntityWithCombatStatistics> newOrder)
    {
        IEnumerable<IGameEntityWithCombatStatistics> intersection = newOrder.Intersect(Entities);
        int intersectionCount = intersection.Count();
        if (intersectionCount != newOrder.Count || intersectionCount != Entities.Count)
        {
            throw new InvalidOperationException("Bad new order");
        }

        List<IGameEntityWithCombatStatistics> oldOrder = _entities;
        _entities = newOrder.ToList();

        Reordered?.Invoke(this, new CombatFormationInPreparationReorderedEvent { OldOrder = oldOrder, NewOrder = _entities });
    }

    public CombatFormation Lock() => new() { Entities = Entities };
}

public class CombatFormationInPreparationReorderedEvent
{
    public required IReadOnlyList<IGameEntityWithCombatStatistics> OldOrder { get; init; }
    public required IReadOnlyList<IGameEntityWithCombatStatistics> NewOrder { get; init; }
}
