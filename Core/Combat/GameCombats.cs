using System.Collections;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Extensions;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Combat;

public class GameCombats : IEnumerable<CombatInstance>, IDisposable
{
    readonly GameSettings _settings;
    readonly GameState _state;
    readonly ILogger<GameCombats> _logger;
    readonly Dictionary<CombatInstanceId, CombatInPreparation> _combatsInPreparation = new();
    readonly Dictionary<CombatInstanceId, CombatInstance> _combats = new();

    public GameCombats(GameState state)
    {
        _settings = state.Settings;
        _state = state;
        _logger = state.LoggerFactory.CreateLogger<GameCombats>();
    }

    public IEnumerable<CombatInPreparation> InPreparation => _combatsInPreparation.Values;

    public IEnumerator<CombatInstance> GetEnumerator() => _combats.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_combats.Values).GetEnumerator();

    public Maybe CanStartCombat(IReadOnlyList<IGameEntityWithCombatStatistics> attackers, IReadOnlyList<IGameEntityWithCombatStatistics> defenders)
    {
        if (defenders.Any(target => target.Busy))
        {
            return "Target is busy";
        }

        return true;
    }

    public Maybe CanJoinCombat(IGameEntityWithCombatStatistics entity, CombatInPreparation combat, CombatSide side) => combat.GetTeam(side).CanJoin(entity);

    public async Task<Maybe<CombatInPreparation>> StartCombatPreparationAsync(
        IReadOnlyList<IGameEntityWithCombatStatistics> attackers,
        IReadOnlyList<IGameEntityWithCombatStatistics> defenders
    )
    {
        Maybe canStartCombat = CanStartCombat(attackers, defenders);
        if (!canStartCombat.Success)
        {
            return canStartCombat.WhyNot;
        }

        CombatInPreparation combatInPreparation = new(attackers, defenders, _settings);
        _combatsInPreparation[combatInPreparation.Id] = combatInPreparation;

        await _state.Publisher.Publish(new CombatPreparationStarted { Combat = combatInPreparation });

        combatInPreparation.Attackers.Added += (_, entity) =>
            _state.Publisher.PublishSync(new CombatInPreparationEntityAdded { CombatInPreparation = combatInPreparation, Side = CombatSide.Attackers, Entity = entity });
        combatInPreparation.Attackers.Removed += (_, entity) =>
            _state.Publisher.PublishSync(new CombatInPreparationEntityRemoved { CombatInPreparation = combatInPreparation, Side = CombatSide.Attackers, Entity = entity });
        combatInPreparation.Attackers.Reordered += (_, args) => _state.Publisher.PublishSync(
            new CombatInPreparationEntitiesReordered { CombatInPreparation = combatInPreparation, Side = CombatSide.Attackers, OldOrder = args.OldOrder, NewOrder = args.NewOrder }
        );

        combatInPreparation.Defenders.Added += (_, entity) =>
            _state.Publisher.PublishSync(new CombatInPreparationEntityAdded { CombatInPreparation = combatInPreparation, Side = CombatSide.Defenders, Entity = entity });
        combatInPreparation.Defenders.Removed += (_, entity) =>
            _state.Publisher.PublishSync(new CombatInPreparationEntityRemoved { CombatInPreparation = combatInPreparation, Side = CombatSide.Defenders, Entity = entity });
        combatInPreparation.Defenders.Reordered += (_, args) => _state.Publisher.PublishSync(
            new CombatInPreparationEntitiesReordered { CombatInPreparation = combatInPreparation, Side = CombatSide.Defenders, OldOrder = args.OldOrder, NewOrder = args.NewOrder }
        );

        return combatInPreparation;
    }

    public async Task<Maybe<CombatInstance>> StartCombatAsync(CombatInPreparation combatInPreparation)
    {
        CombatInstance combat = combatInPreparation.Start();

        await _state.Publisher.Publish(new CombatStarted { Combat = combat });

        combat.Attacked += (_, args) => _state.Publisher.PublishSync(
            new CombatEntityAttacked
            {
                Combat = combat,
                SubTurn = args.SubTurn,
                Attacker = args.Attacker,
                Target = args.Target,
                AttackDealt = args.AttackDealt,
                AttackReceived = args.AttackReceived
            }
        );

        _combats.Add(combat.Id, combat);
        _combatsInPreparation.Remove(combatInPreparation.Id);

        return combat;
    }

    public void RemoveCombat(CombatInstance combat)
    {
        if (_combats.Remove(combat.Id))
        {
            combat.Dispose();
        }
    }

    public CombatInPreparation? GetCombatInPreparation(CombatInstanceId combatInstanceId) => _combatsInPreparation.GetValueOrDefault(combatInstanceId);
    public CombatInstance? GetCombat(CombatInstanceId combatInstanceId) => _combats.GetValueOrDefault(combatInstanceId);

    public IEnumerable<CombatInPreparation> GetCombatInPreparationAtLocation(Location location) => _combatsInPreparation.Values.Where(c => c.Location == location);
    public IEnumerable<CombatInstance> GetCombatAtLocation(Location location) => _combats.Values.Where(c => c.Location == location);

    public void Dispose()
    {
        foreach (CombatInstance combat in _combats.Values)
        {
            combat.Dispose();
        }
        _combats.Clear();

        GC.SuppressFinalize(this);
    }
}
