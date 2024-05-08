using System.Diagnostics.CodeAnalysis;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Settings;

namespace RestAdventure.Core.Combat;

public record CombatInstanceId(Guid Guid);

public class CombatInstance : IDisposable
{
    readonly GameSettings _settings;
    readonly Dictionary<GameEntityId, EntityState> _states;

    internal CombatInstance(CombatInPreparation combatInPreparation)
    {
        Id = combatInPreparation.Id;
        Location = combatInPreparation.Location;
        CombatFormation team1 = combatInPreparation.Team1.Lock();
        CombatFormation team2 = combatInPreparation.Team2.Lock();

        if (!team1.Entities.Any(e => e.Combat.Health > 0))
        {
            throw new ArgumentException("Team 1 cannot be empty", nameof(combatInPreparation));
        }

        if (!team2.Entities.Any(e => e.Combat.Health > 0))
        {
            throw new ArgumentException("Team 2 cannot be empty", nameof(combatInPreparation));
        }

        _settings = combatInPreparation.Settings;

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

    public CombatInstanceId Id { get; }
    public Location Location { get; }
    public CombatFormation Team1 { get; }
    public CombatFormation Team2 { get; }

    public int Turn { get; private set; }

    [MemberNotNullWhen(true, nameof(Winner))]
    public bool IsOver { get; private set; }

    public CombatSide? Winner { get; private set; }

    public event EventHandler<CombatEntityAttackedEvent>? Attacked;

    IEnumerable<EntityState> Alive => _states.Values.Where(s => s.Entity.Combat.Health > 0);

    public async Task PlayTurnAsync()
    {
        foreach (EntityState entityState in Alive)
        {
            entityState.Lead += entityState.Entity.Combat.Speed;
        }

        int subTurn = 1;
        while (true)
        {
            EntityState? next = Alive.Where(s => s.Lead > 0).MaxBy(s => s.Lead);
            if (next == null)
            {
                break;
            }

            CombatFormation otherFormation = GetTeam(next.Team.OtherSide());
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

    public CombatFormation GetTeam(CombatSide team) =>
        team switch
        {
            CombatSide.Team1 => Team1,
            CombatSide.Team2 => Team2,
            _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
        };

    Task ResolveAttackAsync(int subTurn, IGameEntityWithCombatStatistics attacker, IGameEntityWithCombatStatistics target)
    {
        EntityAttack damageToDeal = attacker.Combat.DealAttack();
        EntityAttack damageReceived = target.Combat.ReceiveAttack(damageToDeal);

        Attacked?.Invoke(
            this,
            new CombatEntityAttackedEvent { SubTurn = subTurn, Attacker = attacker, Target = target, AttackDealt = damageToDeal, AttackReceived = damageReceived }
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
    public required EntityAttack AttackDealt { get; init; }
    public required EntityAttack AttackReceived { get; init; }
}
