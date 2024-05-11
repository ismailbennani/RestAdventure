using RestAdventure.Core.Combat.Options;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Combat;

public class CombatFormation
{
    readonly List<IGameEntityWithCombatCapabilities> _entities;

    public CombatFormation(CombatInstance combat, IEnumerable<IGameEntityWithCombatCapabilities> entities, CombatFormationOptions options)
    {
        Combat = combat;
        Options = options;
        _entities = entities.ToList();
        Owner = _entities[0];
    }

    public CombatInstance Combat { get; }

    /// <summary>
    ///     The owner of the formation, they are the one that can change the options
    /// </summary>
    public IGameEntityWithCombatCapabilities Owner { get; }

    /// <summary>
    ///     The characters information
    /// </summary>
    public IEnumerable<IGameEntityWithCombatCapabilities> Entities => _entities;

    /// <summary>
    ///     The options of the formation
    /// </summary>
    public CombatFormationOptions Options { get; private set; }

    public event EventHandler<IGameEntityWithCombatCapabilities>? Added;
    public event EventHandler<IGameEntityWithCombatCapabilities>? Removed;

    public Maybe CanAdd(IGameEntityWithCombatCapabilities entity)
    {
        if (Combat.Phase != CombatPhase.Preparation)
        {
            return "Combat is no longer in the preparation phase";
        }

        if (Options.Accessibility == CombatFormationAccessibility.TeamOnly && Owner.Team != entity.Team)
        {
            return "Team locked";
        }

        if (_entities.Count >= Options.MaxCount)
        {
            return "Team full";
        }

        return true;
    }

    public Maybe SetOptions(IGameEntityWithCombatCapabilities entity, CombatFormationOptions options)
    {
        if (Combat.Phase != CombatPhase.Preparation)
        {
            return "Combat is no longer in the preparation phase";
        }

        if (entity != Owner)
        {
            return "Only the owner can change the options";
        }

        Options = options;

        switch (Options.Accessibility)
        {
            case CombatFormationAccessibility.TeamOnly:
                foreach (IGameEntityWithCombatCapabilities entityInCombat in _entities.ToArray())
                {
                    if (entityInCombat.Team != Owner.Team)
                    {
                        _entities.Remove(entityInCombat);
                    }
                }
                break;
        }

        return true;
    }

    public Maybe Add(IGameEntityWithCombatCapabilities entity)
    {
        Maybe canJoin = CanAdd(entity);
        if (!canJoin.Success)
        {
            return canJoin;
        }

        _entities.Add(entity);

        Added?.Invoke(this, entity);
        return true;
    }

    public Maybe Remove(IGameEntityWithCombatCapabilities entity)
    {
        if (Combat.Phase != CombatPhase.Preparation)
        {
            return "Combat is no longer in the preparation phase";
        }

        if (entity == Owner)
        {
            return "Cannot remove the owner of the formation";
        }

        if (!_entities.Remove(entity))
        {
            return "Cannot remove entity because it is not part of the formation";
        }

        Removed?.Invoke(this, entity);
        return true;
    }
}
