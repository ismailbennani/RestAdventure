using MediatR;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Extensions;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Settings;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Combat;

public class GameCombats : IDisposable
{
    readonly GameSettings _settings;
    readonly IPublisher _publisher;
    readonly ILogger<GameCombats> _logger;
    readonly Dictionary<CombatInstanceId, CombatInPreparation> _combatsInPreparation = new();
    readonly Dictionary<CombatInstanceId, CombatInstance> _combats = new();

    public GameCombats(GameSettings settings, IPublisher publisher, ILogger<GameCombats> logger)
    {
        _settings = settings;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<CombatInPreparation> StartCombatAsync(IGameEntityWithCombatStatistics attacker, IGameEntityWithCombatStatistics target)
    {
        CombatInPreparation combatInPreparation = new(attacker, target, _settings);
        _combatsInPreparation[combatInPreparation.Id] = combatInPreparation;

        await _publisher.Publish(new CombatPreparationStarted { Combat = combatInPreparation });

        combatInPreparation.Team1.Added += (_, entity) =>
            _publisher.PublishSync(new CombatInPreparationEntityAdded { CombatInPreparation = combatInPreparation, Side = CombatSide.Team1, Entity = entity });
        combatInPreparation.Team1.Removed += (_, entity) =>
            _publisher.PublishSync(new CombatInPreparationEntityRemoved { CombatInPreparation = combatInPreparation, Side = CombatSide.Team1, Entity = entity });
        combatInPreparation.Team1.Reordered += (_, args) => _publisher.PublishSync(
            new CombatInPreparationEntitiesReordered { CombatInPreparation = combatInPreparation, Side = CombatSide.Team1, OldOrder = args.OldOrder, NewOrder = args.NewOrder }
        );

        combatInPreparation.Team2.Added += (_, entity) =>
            _publisher.PublishSync(new CombatInPreparationEntityAdded { CombatInPreparation = combatInPreparation, Side = CombatSide.Team2, Entity = entity });
        combatInPreparation.Team2.Removed += (_, entity) =>
            _publisher.PublishSync(new CombatInPreparationEntityRemoved { CombatInPreparation = combatInPreparation, Side = CombatSide.Team2, Entity = entity });
        combatInPreparation.Team2.Reordered += (_, args) => _publisher.PublishSync(
            new CombatInPreparationEntitiesReordered { CombatInPreparation = combatInPreparation, Side = CombatSide.Team2, OldOrder = args.OldOrder, NewOrder = args.NewOrder }
        );

        return combatInPreparation;
    }

    public async Task<Maybe> CancelCombatAsync(CombatInstanceId combatInstanceId)
    {
        if (_combatsInPreparation.Remove(combatInstanceId, out CombatInPreparation? combatInPreparation))
        {
            combatInPreparation.Cancel();
            await _publisher.Publish(new CombatInPreparationCanceled { Combat = combatInPreparation });
            return true;
        }

        if (_combats.ContainsKey(combatInstanceId))
        {
            return "Cannot cancel combat past the preparation phase.";
        }

        return "Combat not found";
    }

    public async Task ResolveCombatsAsync(GameState state)
    {
        await ResolveCombatsInPreparationAsync();
        await ResolveCombatsAsync();
    }

    async Task ResolveCombatsInPreparationAsync()
    {
        List<CombatInstanceId> toRemove = [];
        foreach (CombatInPreparation combatInPreparation in _combatsInPreparation.Values)
        {
            combatInPreparation.Tick();

            if (combatInPreparation.Turn >= _settings.Combat.PreparationPhaseDuration)
            {
                CombatInstance combat = new(combatInPreparation);
                _combats[combat.Id] = combat;
                await _publisher.Publish(new CombatStarted { Combat = combat });

                combat.Attacked += (_, args) => _publisher.PublishSync(
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

                toRemove.Add(combatInPreparation.Id);
            }
        }

        foreach (CombatInstanceId combatInPreparationId in toRemove)
        {
            _combatsInPreparation.Remove(combatInPreparationId);
        }
    }

    async Task ResolveCombatsAsync()
    {
        List<CombatInstanceId> toRemove = [];
        foreach (CombatInstance combat in _combats.Values)
        {
            await combat.PlayTurnAsync();

            if (combat.IsOver)
            {
                toRemove.Add(combat.Id);
            }
        }

        foreach (CombatInstanceId combatId in toRemove)
        {
            if (!_combats.Remove(combatId, out CombatInstance? combat))
            {
                _logger.LogWarning("Could not remove combat {id}", combatId);
                continue;
            }

            await _publisher.Publish(new CombatEnded { Combat = combat });

            combat.Dispose();
        }
    }

    public CombatInPreparation? GetInPreparation(CombatInstanceId combatInstanceId) => _combatsInPreparation.GetValueOrDefault(combatInstanceId);

    public CombatInstance? Get(CombatInstanceId combatInstanceId) => _combats.GetValueOrDefault(combatInstanceId);

    public IEnumerable<CombatInstance> AtLocation(Location location) => _combats.Values.Where(c => c.Location == location);

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
