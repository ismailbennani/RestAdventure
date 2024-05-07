using RestAdventure.Core.Entities;
using RestAdventure.Core.Settings;

namespace RestAdventure.Core.Combat;

public record CombatInstanceId(Guid Guid);

public class CombatInstance : IDisposable
{
    readonly GameSettings _settings;
    readonly Dictionary<GameEntityId, EntityState> _states;

    internal CombatInstance(CombatFormation team1, CombatFormation team2, GameSettings settings)
    {
        if (!team1.Entities.Any(e => e.Combat.Health > 0))
        {
            throw new ArgumentException("Team 1 cannot be empty", nameof(team1));
        }

        if (!team2.Entities.Any(e => e.Combat.Health > 0))
        {
            throw new ArgumentException("Team 2 cannot be empty", nameof(team2));
        }
        _settings = settings;

        Team1 = team1;
        Team2 = team2;

        Turn = 1;
        _states = new Dictionary<GameEntityId, EntityState>();

        foreach (IGameEntityWithCombatStatistics entity in team1.Entities)
        {
            _states[entity.Id] = new EntityState(entity, CombatSide.Team1);
        }

        foreach (IGameEntityWithCombatStatistics entity in team2.Entities)
        {
            _states[entity.Id] = new EntityState(entity, CombatSide.Team2);
        }
    }

    public CombatInstanceId Id { get; } = new(Guid.NewGuid());
    public CombatFormation Team1 { get; }
    public CombatFormation Team2 { get; }

    public int Turn { get; private set; }
    public bool IsOver { get; private set; }
    public CombatSide? Winner { get; private set; }

    public event EventHandler<CombatEntityAttackedEvent>? Attacked;

    public async Task PlayTurnAsync()
    {
        foreach (EntityState entityState in _states.Values)
        {
            entityState.Lead += entityState.Entity.Combat.Speed;
        }

        int subTurn = 1;
        while (true)
        {
            EntityState? next = _states.Values.Where(s => s.Lead > 0).MaxBy(s => s.Lead);
            if (next == null)
            {
                break;
            }

            CombatFormation otherFormation = GetFormation(next.Team.OtherSide());
            IGameEntityWithCombatStatistics target = otherFormation.Entities[0];

            await ResolveAttackAsync(subTurn, next.Entity, target);

            if (EvaluateWinCondition())
            {
                return;
            }

            next.Lead -= _settings.Combat.CombatTurnDuration;

            subTurn++;
        }

        Turn++;
    }

    Task ResolveAttackAsync(int subTurn, IGameEntityWithCombatStatistics attacker, IGameEntityWithCombatStatistics target)
    {
        int damageToDeal = attacker.Combat.Attack();
        int damageReceived = target.Combat.ReceiveAttack(damageToDeal);

        Attacked?.Invoke(
            this,
            new CombatEntityAttackedEvent { SubTurn = subTurn, Attacker = attacker, Target = target, DamageDealt = damageToDeal, DamageReceived = damageReceived }
        );
        
        return Task.CompletedTask;
    }

    bool EvaluateWinCondition()
    {
        if (HasLost(Team1))
        {
            Winner = CombatSide.Team2;
            IsOver = true;
            return true;
        }

        if (HasLost(Team2))
        {
            Winner = CombatSide.Team1;
            IsOver = true;
            return true;
        }

        return false;
    }

    static bool HasLost(CombatFormation team) => team.Entities.All(c => c.Combat.Health <= 0);

    CombatFormation GetFormation(CombatSide team) =>
        team switch
        {
            CombatSide.Team1 => Team1,
            CombatSide.Team2 => Team2,
            _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
        };

    class EntityState
    {
        public EntityState(IGameEntityWithCombatStatistics entity, CombatSide team)
        {
            Entity = entity;
            Team = team;
        }

        public IGameEntityWithCombatStatistics Entity { get; }
        public CombatSide Team { get; }
        public int Lead { get; set; }
    }

    public void Dispose()
    {
        Attacked = null;
        GC.SuppressFinalize(this);
    }
}

public class CombatEntityAttackedEvent
{
    public required int SubTurn { get; init; }
    public required IGameEntityWithCombatStatistics Attacker { get; init; }
    public required IGameEntityWithCombatStatistics Target { get; init; }
    public required int DamageDealt { get; init; }
    public required int DamageReceived { get; init; }
}
