using Microsoft.Extensions.Logging;
using RestAdventure.Core.Simulation;

namespace RestAdventure.Core.Combat;

public class CombatSimulationWorkProvider : SimulationWorkProvider
{
    readonly GameState _state;
    readonly ILogger<CombatSimulationWorkProvider> _logger;

    public CombatSimulationWorkProvider(GameState state)
    {
        _state = state;
        _logger = state.LoggerFactory.CreateLogger<CombatSimulationWorkProvider>();
    }

    public override IEnumerable<GameSimulation.Work> Tick()
    {
        foreach (CombatInstance combat in _state.Combats)
        {
            yield return new GameSimulation.Work(
                combat.Id.Guid,
                () =>
                {
                    combat.Tick();
                    return Task.CompletedTask;
                }
            );
        }
    }

    public override IEnumerable<GameSimulation.Work> Late()
    {
        yield return new GameSimulation.Work(
            default,
            () =>
            {
                _state.Combats.RemoveCombatsInPhaseEnd();
                return Task.CompletedTask;
            }
        );
    }
}
