using RestAdventure.Core.Combat.Options;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Combat;

public record CombatInstanceId(Guid Guid);

public class CombatInstance
{
    readonly Dictionary<IGameEntityWithCombatCapabilities, IReadOnlyCollection<EntityState>> _entities = new();
    CombatFormationOptions? _attackersOptionsToSet;
    CombatFormationOptions? _defendersOptionsToSet;

    public CombatInstance(
        IEnumerable<IGameEntityWithCombatCapabilities> attackers,
        CombatFormationOptions attackersOptions,
        IEnumerable<IGameEntityWithCombatCapabilities> defenders,
        CombatFormationOptions defendersOptions,
        CombatOptions options
    )
    {
        Options = options;
        Attackers = new CombatFormation(this, attackers, attackersOptions);
        Defenders = new CombatFormation(this, defenders, defendersOptions);
        Phase = CombatPhase.Preparation;
        Location = Attackers.Owner.Location;
    }

    public CombatInstanceId Id { get; } = new(Guid.NewGuid());
    public CombatPhase Phase { get; private set; }
    public int Turn { get; private set; }
    public Location Location { get; set; }
    public CombatOptions Options { get; }
    public CombatFormation Attackers { get; private set; }
    public CombatFormation Defenders { get; private set; }

    public IReadOnlyList<ICombatEntity> AttackerCombatEntities =>
        _entities.Values.SelectMany(e => e).Where(e => e.Side == CombatSide.Attackers).OrderBy(e => e.Position).Select(e => e.Entity).ToArray();

    public IReadOnlyList<ICombatEntity> DefenderCombatEntities =>
        _entities.Values.SelectMany(e => e).Where(e => e.Side == CombatSide.Defenders).OrderBy(e => e.Position).Select(e => e.Entity).ToArray();

    public CombatSide? Winner { get; private set; }

    public event EventHandler? Started;
    public event EventHandler<CombatEntityAttackedEvent>? CombatEntityAttacked;
    public event EventHandler<CombatEntityDiedEvent>? CombatEntityDied;
    public event EventHandler? Ended;

    public Maybe Tick()
    {
        switch (Phase)
        {
            case CombatPhase.Preparation:
                TickInPreparation();
                return true;
            case CombatPhase.Combat:
                TickInCombat();
                return true;
            case CombatPhase.End:
                return "Combat is over";
            default:
                throw new ArgumentOutOfRangeException(nameof(Phase), Phase, null);
        }
    }

    public CombatFormation GetTeam(CombatSide side) =>
        side switch
        {
            CombatSide.Attackers => Attackers,
            CombatSide.Defenders => Defenders,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };

    void TickInPreparation()
    {
        Turn++;

        if (_attackersOptionsToSet != null)
        {
            Attackers.SetOptions(Attackers.Owner, _attackersOptionsToSet);
            _attackersOptionsToSet = null;
        }

        if (_defendersOptionsToSet != null)
        {
            Defenders.SetOptions(Defenders.Owner, _defendersOptionsToSet);
            _defendersOptionsToSet = null;
        }

        if (Turn >= Options.PreparationPhaseDuration)
        {
            Start();
        }
    }

    void TickInCombat()
    {
        Turn++;

        List<EntityState> attackerEntities = Attackers.Entities.SelectMany(a => _entities[a]).Where(a => a.Entity.Health > 0).OrderBy(c => c.Position).ToList();
        List<EntityState> defenderEntities = Defenders.Entities.SelectMany(a => _entities[a]).Where(a => a.Entity.Health > 0).OrderBy(c => c.Position).ToList();

        foreach (EntityState entityState in attackerEntities.Concat(defenderEntities))
        {
            entityState.Lead += entityState.Entity.Speed;
        }

        int subTurn = 0;
        while (true)
        {
            EntityState? next = attackerEntities.Concat(defenderEntities).Where(s => s.Lead > 0).MaxBy(s => s.Lead);
            if (next == null)
            {
                break;
            }

            List<EntityState> otherTeam = next.Side switch
            {
                CombatSide.Attackers => defenderEntities,
                CombatSide.Defenders => attackerEntities,
                _ => throw new ArgumentOutOfRangeException()
            };

            EntityState target = otherTeam.First();

            CombatEntityAttack damageToDeal = next.Entity.DealAttack();
            CombatEntityAttack damageReceived = target.Entity.ReceiveAttack(damageToDeal);

            CombatEntityAttacked?.Invoke(
                this,
                new CombatEntityAttackedEvent { SubTurn = subTurn, Attacker = next.Entity, Target = target.Entity, AttackDealt = damageToDeal, AttackReceived = damageReceived }
            );

            foreach (EntityState entity in attackerEntities.ToArray())
            {
                if (entity.Entity.Health <= 0)
                {
                    attackerEntities.Remove(entity);
                    CombatEntityDied?.Invoke(this, new CombatEntityDiedEvent { SubTurn = subTurn, Attacker = next.Entity, Entity = entity.Entity });
                }
            }

            if (attackerEntities.Count == 0)
            {
                End(CombatSide.Defenders);
                break;
            }

            foreach (EntityState entity in defenderEntities.ToArray())
            {
                if (entity.Entity.Health <= 0)
                {
                    defenderEntities.Remove(entity);
                    CombatEntityDied?.Invoke(this, new CombatEntityDiedEvent { SubTurn = subTurn, Attacker = next.Entity, Entity = entity.Entity });
                }
            }
            if (defenderEntities.Count == 0)
            {
                End(CombatSide.Attackers);
                break;
            }

            next.Lead -= Options.CombatTurnDuration;
            subTurn++;
        }
    }

    void Start()
    {
        foreach (IGameEntityWithCombatCapabilities gameEntity in Attackers.Entities)
        {
            IEnumerable<ICombatEntity> combatEntities = gameEntity.SpawnCombatEntities();
            _entities[gameEntity] = combatEntities.Select(e => new EntityState(e, CombatSide.Attackers)).ToList();
        }

        foreach (IGameEntityWithCombatCapabilities gameEntity in Defenders.Entities)
        {
            IEnumerable<ICombatEntity> combatEntities = gameEntity.SpawnCombatEntities();
            _entities[gameEntity] = combatEntities.Select(e => new EntityState(e, CombatSide.Defenders)).ToList();
        }

        Phase = CombatPhase.Combat;
        Turn = 0;

        Started?.Invoke(this, EventArgs.Empty);
    }

    void End(CombatSide winner)
    {
        Winner = winner;
        Phase = CombatPhase.End;

        foreach ((IGameEntityWithCombatCapabilities gameEntity, IReadOnlyCollection<EntityState> entities) in _entities)
        {
            gameEntity.DestroyCombatEntities(entities.Select(e => e.Entity));
        }

        Ended?.Invoke(this, EventArgs.Empty);
    }

    class EntityState
    {
        public EntityState(ICombatEntity entity, CombatSide side)
        {
            Entity = entity;
            Side = side;
        }

        public ICombatEntity Entity { get; }
        public CombatSide Side { get; }
        public int Position { get; set; }
        public int Lead { get; set; }
    }
}

public class CombatEntityAttackedEvent
{
    public required int SubTurn { get; init; }
    public required ICombatEntity Attacker { get; init; }
    public required ICombatEntity Target { get; init; }
    public required CombatEntityAttack AttackDealt { get; init; }
    public required CombatEntityAttack AttackReceived { get; init; }
}

public class CombatEntityDiedEvent
{
    public required int SubTurn { get; init; }
    public required ICombatEntity Attacker { get; init; }
    public required ICombatEntity Entity { get; init; }
}
