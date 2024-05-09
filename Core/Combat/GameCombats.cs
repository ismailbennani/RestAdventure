using MediatR;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Extensions;
using RestAdventure.Core.Items;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Combat;

public class GameCombats : IDisposable
{
    readonly GameSettings _settings;
    readonly IPublisher _publisher;
    readonly GameState _state;
    readonly ILogger<GameCombats> _logger;
    readonly Dictionary<CombatInstanceId, CombatInPreparation> _combatsInPreparation = new();
    readonly Dictionary<CombatInstanceId, CombatInstance> _combats = new();

    public GameCombats(GameState state, ILogger<GameCombats> logger)
    {
        _settings = state.Settings;
        _publisher = state.Publisher;
        _state = state;
        _logger = logger;
    }

    public Maybe CanStartCombat(IReadOnlyList<IGameEntityWithCombatStatistics> attackers, IReadOnlyList<IGameEntityWithCombatStatistics> defenders)
    {
        if (defenders.Any(target => target.Busy))
        {
            return "Target is busy";
        }

        return true;
    }

    public Maybe CanJoinCombat(IGameEntityWithCombatStatistics entity, CombatInPreparation combat, CombatSide side) => combat.GetTeam(side).CanJoin(entity);

    public async Task<Maybe<CombatInPreparation>> StartCombatAsync(
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

        await _publisher.Publish(new CombatPreparationStarted { Combat = combatInPreparation });

        combatInPreparation.Attackers.Added += (_, entity) =>
            _publisher.PublishSync(new CombatInPreparationEntityAdded { CombatInPreparation = combatInPreparation, Side = CombatSide.Attackers, Entity = entity });
        combatInPreparation.Attackers.Removed += (_, entity) =>
            _publisher.PublishSync(new CombatInPreparationEntityRemoved { CombatInPreparation = combatInPreparation, Side = CombatSide.Attackers, Entity = entity });
        combatInPreparation.Attackers.Reordered += (_, args) => _publisher.PublishSync(
            new CombatInPreparationEntitiesReordered { CombatInPreparation = combatInPreparation, Side = CombatSide.Attackers, OldOrder = args.OldOrder, NewOrder = args.NewOrder }
        );

        combatInPreparation.Defenders.Added += (_, entity) =>
            _publisher.PublishSync(new CombatInPreparationEntityAdded { CombatInPreparation = combatInPreparation, Side = CombatSide.Defenders, Entity = entity });
        combatInPreparation.Defenders.Removed += (_, entity) =>
            _publisher.PublishSync(new CombatInPreparationEntityRemoved { CombatInPreparation = combatInPreparation, Side = CombatSide.Defenders, Entity = entity });
        combatInPreparation.Defenders.Reordered += (_, args) => _publisher.PublishSync(
            new CombatInPreparationEntitiesReordered { CombatInPreparation = combatInPreparation, Side = CombatSide.Defenders, OldOrder = args.OldOrder, NewOrder = args.NewOrder }
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
        await ResolveCombatsAsync();
        await ResolveCombatsInPreparationAsync();
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

            await ResolveEndOfCombatAsync(combat);

            await _publisher.Publish(new CombatEnded { Combat = combat });

            combat.Dispose();
        }
    }

    public CombatInPreparation? GetInPreparation(CombatInstanceId combatInstanceId) => _combatsInPreparation.GetValueOrDefault(combatInstanceId);
    public CombatInstance? Get(CombatInstanceId combatInstanceId) => _combats.GetValueOrDefault(combatInstanceId);

    public IEnumerable<CombatInPreparation> InPreparationAtLocation(Location location) => _combatsInPreparation.Values.Where(c => c.Location == location);
    public IEnumerable<CombatInstance> AtLocation(Location location) => _combats.Values.Where(c => c.Location == location);

    async Task ResolveEndOfCombatAsync(CombatInstance combat)
    {
        if (!combat.Winner.HasValue)
        {
            return;
        }

        CombatFormation winnerTeam = combat.GetTeam(combat.Winner.Value);
        CombatFormation loserTeam = combat.GetTeam(combat.Winner.Value.OtherSide());

        int baseExperience = loserTeam.Entities.OfType<MonsterInstance>().Sum(m => m.Species.Experience);
        ItemStack[] loot = loserTeam.Entities.OfType<MonsterInstance>()
            .Aggregate(Enumerable.Empty<ItemStack>(), (items, m) => items.Concat(m.Species.Items.Concat(m.Species.Family.Items)))
            .ToArray();

        IEnumerable<Character> characters = winnerTeam.Entities.OfType<Character>();
        foreach (Character character in characters)
        {
            if (baseExperience > 0)
            {
                character.Progression.Progress(baseExperience);
            }

            character.Inventory.Add(loot);
        }

        foreach (IGameEntityWithCombatStatistics entity in loserTeam.Entities)
        {
            await entity.KillAsync(_state);
        }
    }

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
