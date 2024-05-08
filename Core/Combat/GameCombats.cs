﻿using MediatR;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Settings;

namespace RestAdventure.Core.Combat;

public class GameCombats : IDisposable
{
    readonly GameSettings _settings;
    readonly IPublisher _publisher;
    readonly ILogger<GameCombats> _logger;
    readonly Dictionary<CombatInstanceId, CombatInstance> _combats = new();

    public GameCombats(GameSettings settings, IPublisher publisher, ILogger<GameCombats> logger)
    {
        _settings = settings;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<CombatInstance> StartCombatAsync(IGameEntityWithCombatStatistics attacker, IGameEntityWithCombatStatistics target)
    {
        CombatInPreparation combatInPreparation = new(attacker, target, _settings);
        CombatInstance combat = new(combatInPreparation);
        _combats[combat.Id] = combat;
        await _publisher.Publish(new CombatStarted { Combat = combat });

        combat.Attacked += (_, args) => _publisher.Publish(
                new CombatEntityAttacked
                {
                    Combat = combat,
                    SubTurn = args.SubTurn,
                    Attacker = args.Attacker,
                    Target = args.Target,
                    AttackDealt = args.AttackDealt,
                    AttackReceived = args.AttackReceived
                }
            )
            .Wait();

        return combat;
    }

    public async Task RemoveCombatsIfOverAsync(GameState state)
    {
        List<CombatInstanceId> toRemove = [];
        foreach (CombatInstance combat in _combats.Values)
        {
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
