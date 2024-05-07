using RestAdventure.Core.Entities;
using RestAdventure.Core.Settings;

namespace RestAdventure.Core.Combat;

public record CombatInstanceId(Guid Guid);

public class CombatInstance
{
    readonly GameSettings _settings;
    readonly Dictionary<GameEntityId, EntityState> _states;

    internal CombatInstance(CombatFormation team1, CombatFormation team2, GameSettings settings)
    {
        if (team1.Entities.Count(e => e.Combat.Health > 0) == 0)
        {
            throw new ArgumentException("Team 1 cannot be empty", nameof(team1));
        }

        if (team2.Entities.Count(e => e.Combat.Health > 0) == 0)
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
            _states[entity.Id] = new EntityState(entity, Team.Team1);
        }

        foreach (IGameEntityWithCombatStatistics entity in team2.Entities)
        {
            _states[entity.Id] = new EntityState(entity, Team.Team2);
        }
    }

    public CombatInstanceId Id { get; } = new(Guid.NewGuid());
    public CombatFormation Team1 { get; }
    public CombatFormation Team2 { get; }

    public int Turn { get; private set; }
    public bool IsOver { get; private set; }
    public Team? Winner { get; private set; }

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

            CombatFormation otherFormation = GetFormation(OtherTeam(next.Team));
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

    async Task ResolveAttackAsync(int subTurn, IGameEntityWithCombatStatistics attacker, IGameEntityWithCombatStatistics target) => throw new NotImplementedException();

    bool EvaluateWinCondition()
    {
        if (HasLost(Team1))
        {
            Winner = Team.Team2;
            IsOver = true;
            return true;
        }

        if (HasLost(Team2))
        {
            Winner = Team.Team1;
            IsOver = true;
            return true;
        }

        return false;
    }

    static bool HasLost(CombatFormation team) => team.Entities.All(c => c.Combat.Health <= 0);

    CombatFormation GetFormation(Team team) =>
        team switch
        {
            Team.Team1 => Team1,
            Team.Team2 => Team2,
            _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
        };

    Team OtherTeam(Team team) =>
        team switch
        {
            Team.Team1 => Team.Team2,
            Team.Team2 => Team.Team1
        };

    public enum Team
    {
        Team1,
        Team2
    }

    class EntityState
    {
        public EntityState(IGameEntityWithCombatStatistics entity, Team team)
        {
            Entity = entity;
            Team = team;
        }

        public IGameEntityWithCombatStatistics Entity { get; }
        public Team Team { get; }
        public int Lead { get; set; }
    }
}

public class CombatFormation
{
    /// <summary>
    ///     The characters in order
    /// </summary>
    public required IReadOnlyList<IGameEntityWithCombatStatistics> Entities { get; init; }
}

public interface IGameEntityWithCombatStatistics : IGameEntity
{
    EntityCombatStatistics Combat { get; }
}

public class EntityCombatStatistics
{
    public int Health { get; private set; }
    public int Speed { get; private set; }
}
