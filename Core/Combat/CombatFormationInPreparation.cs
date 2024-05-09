using RestAdventure.Core.Combat.Options;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Combat;

public class CombatFormationInPreparation
{
    readonly GameSettings _gameSettings;
    List<IGameEntityWithCombatStatistics> _entities;

    public CombatFormationInPreparation(IReadOnlyList<IGameEntityWithCombatStatistics> entities, CombatFormationOptions options, GameSettings gameSettings)
    {
        Owner = entities[0];
        _entities = entities.ToList();
        CombatEntityKind = Owner.CombatEntityKind;
        Options = options;
        _gameSettings = gameSettings;
    }

    public IGameEntityWithCombatStatistics Owner { get; }

    public CombatFormationOptions Options { get; private set; }

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

    public Maybe CanJoin(IGameEntityWithCombatStatistics entity)
    {
        if (entity.CombatEntityKind != CombatEntityKind)
        {
            return $"Cannot add entity of kind {entity.CombatEntityKind}";
        }

        if (Options.Accessibility == CombatFormationAccessibility.TeamOnly && Owner.Team != entity.Team)
        {
            return "Team locked";
        }

        if (_entities.Count >= MaxCount)
        {
            return "Team full";
        }

        return true;
    }

    public Maybe SetOptions(IGameEntityWithCombatStatistics entity, CombatFormationOptions options)
    {
        if (entity != Owner)
        {
            return "Only the owner can change the options";
        }

        Options = options;
        return true;
    }

    public Maybe Add(IGameEntityWithCombatStatistics entity)
    {
        Maybe canJoin = CanJoin(entity);
        if (!canJoin.Success)
        {
            return canJoin;
        }

        _entities.Add(entity);

        Added?.Invoke(this, entity);
        return true;
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
