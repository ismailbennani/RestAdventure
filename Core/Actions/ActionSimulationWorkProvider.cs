using Microsoft.Extensions.Logging;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Simulation;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Actions;

public class ActionSimulationWorkProvider : SimulationWorkProvider
{
    readonly GameState _state;
    readonly ILogger<ActionSimulationWorkProvider> _logger;

    public ActionSimulationWorkProvider(GameState state)
    {
        _state = state;
        _logger = state.LoggerFactory.CreateLogger<ActionSimulationWorkProvider>();
    }

    public override IEnumerable<GameSimulation.Work> Early()
    {
        foreach ((CharacterId? characterId, Action? _) in _state.Actions.Queued)
        {
            yield return new GameSimulation.Work(characterId.Guid, () => StartQueuedActionAsync(characterId));
        }
    }

    public override IEnumerable<GameSimulation.Work> Tick()
    {
        foreach ((CharacterId characterId, Action action) in _state.Actions.Ongoing)
        {
            yield return new GameSimulation.Work(characterId.Guid, () => TickActionAsync(characterId));
        }
    }

    public override IEnumerable<GameSimulation.Work> Late()
    {
        foreach ((CharacterId characterId, Action action) in _state.Actions.Ongoing)
        {
            if (!action.Over)
            {
                continue;
            }

            yield return new GameSimulation.Work(characterId.Guid, () => RemoveActionAsync(characterId, action));
        }
    }

    public override IEnumerable<GameSimulation.Work> PostTick()
    {
        yield return new GameSimulation.Work(default, PostTickAsync);
    }

    async Task StartQueuedActionAsync(CharacterId characterId)
    {
        Character? character = _state.Entities.Get<Character>(characterId);
        if (character == null)
        {
            _logger.LogError("Character {characterI} not found. Action will be discarded without being started.", characterId);
            return;
        }

        Maybe performed = await _state.Actions.StartQueuedActionAsync(character);
        if (!performed.Success)
        {
            _logger.LogError("Could not perform queued action for character {character}: {reason}", character, performed.WhyNot);
        }
    }

    async Task TickActionAsync(CharacterId characterId)
    {
        Character? character = _state.Entities.Get<Character>(characterId);
        if (character == null)
        {
            return;
        }

        Maybe performed = await _state.Actions.TickOngoingActionAsync(character);
        if (!performed.Success)
        {
            _logger.LogError("Could not perform ongoing action for character {character}: {reason}", character, performed.WhyNot);
        }
    }

    async Task RemoveActionAsync(CharacterId characterId, Action action)
    {
        Character? character = _state.Entities.Get<Character>(characterId);
        if (character == null)
        {
            _logger.LogError("Character {characterId} not found. Action will be removed without calling {onEnd}.", characterId, nameof(Action.EndAsync));
            return;
        }

        Maybe performed = await _state.Actions.EndOngoingActionAsync(character);
        if (!performed.Success)
        {
            _logger.LogError("Could not end ongoing action for character {character}: {reason}", character, performed.WhyNot);
        }
    }

    Task PostTickAsync()
    {
        _state.Actions.ClearTickCache();
        return Task.CompletedTask;
    }
}
