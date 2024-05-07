using MediatR;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Settings;

namespace RestAdventure.Core.Combat;

public class GameCombats
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

    public async Task<CombatInstance> StartCombatAsync(CombatFormation team1, CombatFormation team2)
    {
        CombatInstance combat = new(team1, team2, _settings);
        _combats[combat.Id] = combat;
        await _publisher.Publish(new CombatStarted { Combat = combat });

        combat.Attacked += (_, args) => _publisher.Publish(
                new CombatEntityAttacked
                {
                    Combat = combat,
                    SubTurn = args.SubTurn,
                    Attacker = args.Attacker,
                    Target = args.Target,
                    DamageDealt = args.DamageDealt,
                    DamageReceived = args.DamageReceived
                }
            )
            .Wait();

        return combat;
    }

    public async Task ResolveCombatsAsync(GameState state)
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
        }
    }
}
