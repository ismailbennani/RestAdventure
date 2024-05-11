using System.Collections;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Combat.Options;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Extensions;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Combat;

public class GameCombats : IEnumerable<CombatInstance>
{
    readonly GameState _state;
    readonly ILogger<GameCombats> _logger;
    readonly Dictionary<CombatInstanceId, CombatInstance> _combats = new();

    public GameCombats(GameState state)
    {
        _state = state;
        _logger = state.LoggerFactory.CreateLogger<GameCombats>();
    }

    public IEnumerable<CombatInstance> InPreparation => _combats.Values.Where(c => c.Phase == CombatPhase.Preparation);
    public IEnumerable<CombatInstance> Ongoing => _combats.Values.Where(c => c.Phase == CombatPhase.Combat);

    public IEnumerator<CombatInstance> GetEnumerator() => _combats.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_combats.Values).GetEnumerator();

    public Maybe CanStartCombat(IReadOnlyCollection<IGameEntityWithCombatCapabilities> attackers, IReadOnlyCollection<IGameEntityWithCombatCapabilities> defenders)
    {
        if (defenders.Any(target => target.Busy))
        {
            return "Target is busy";
        }

        return true;
    }

    public async Task<Maybe<CombatInstance>> StartCombatAsync(
        IReadOnlyCollection<IGameEntityWithCombatCapabilities> attackers,
        CombatFormationOptions attackersOptions,
        IReadOnlyCollection<IGameEntityWithCombatCapabilities> defenders,
        CombatFormationOptions defendersOptions
    )
    {
        Maybe canStart = CanStartCombat(attackers, defenders);
        if (!canStart.Success)
        {
            return canStart.WhyNot;
        }

        CombatInstance combat = new(
            attackers,
            attackersOptions,
            defenders,
            defendersOptions,
            new CombatOptions { CombatTurnDuration = _state.Settings.Combat.CombatTurnDuration, PreparationPhaseDuration = _state.Settings.Combat.PreparationPhaseDuration }
        );

        await _state.Publisher.Publish(new CombatPreparationStarted { Combat = combat });

        combat.Started += OnCombatStarted;
        combat.CombatEntityAttacked += OnCombatEntityAttacked;
        combat.CombatEntityDied += OnCombatEntityDied;
        combat.Ended += OnCombatEnded;

        _combats.Add(combat.Id, combat);

        return combat;
    }

    public void RemoveCombatsInPhaseEnd()
    {
        CombatInstanceId[] toRemove = _combats.Values.Where(c => c.Phase == CombatPhase.End).Select(c => c.Id).ToArray();
        foreach (CombatInstanceId combatId in toRemove)
        {
            _combats.Remove(combatId);
        }
    }

    public CombatInstance? GetCombat(CombatInstanceId combatInstanceId) => _combats.GetValueOrDefault(combatInstanceId);

    public IEnumerable<CombatInstance> GetCombatAtLocation(Location location) => _combats.Values.Where(c => c.Location == location);

    public IEnumerable<CombatInstance> GetCombatInPreparationAtLocation(Location location) => InPreparation.Where(c => c.Location == location);

    public IEnumerable<CombatInstance> GetOngoingCombatAtLocation(Location location) => Ongoing.Where(c => c.Location == location);

    public CombatInstance? GetCombatInvolving(IGameEntity gameEntity) =>
        _combats.Values.SingleOrDefault(c => c.Attackers.Entities.Contains(gameEntity) || c.Defenders.Entities.Contains(gameEntity));

    void OnCombatStarted(object? c, EventArgs _)
    {
        if (c is CombatInstance combat)
        {
            _state.Publisher.PublishSync(new CombatStarted { Combat = combat });
        }
    }

    void OnCombatEntityAttacked(object? c, CombatEntityAttackedEvent args)
    {
        if (c is CombatInstance combat)
        {
            _state.Publisher.PublishSync(
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
        }
    }

    void OnCombatEntityDied(object? c, CombatEntityDiedEvent args)
    {
        if (c is CombatInstance combat)
        {
            _state.Publisher.PublishSync(new CombatEntityDied { Combat = combat, SubTurn = args.SubTurn, Attacker = args.Attacker, Entity = args.Entity });
        }
    }

    void OnCombatEnded(object? c, EventArgs _)
    {
        if (c is CombatInstance combat)
        {
            _state.Publisher.PublishSync(new CombatEnded { Combat = combat });
        }
    }
}
