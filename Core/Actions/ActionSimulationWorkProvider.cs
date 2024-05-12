using Microsoft.Extensions.Logging;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Simulation;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Actions;

public class ActionSimulationWorkProvider : SimulationWorkProvider
{
    readonly Game _state;
    readonly ILogger<ActionSimulationWorkProvider> _logger;

    public ActionSimulationWorkProvider(Game state)
    {
        _state = state;
        _logger = state.LoggerFactory.CreateLogger<ActionSimulationWorkProvider>();
    }

    public override IEnumerable<GameSimulation.Work> Early()
    {
        foreach ((CharacterId characterId, Action _) in _state.Actions.Queued)
        {
            Maybe<Character> character = GetCharacter(characterId);
            if (!character.Success)
            {
                _logger.LogError("Character {characterI} not found. Action will be discarded without being started.", characterId);
                continue;
            }

            yield return new GameSimulation.Work(characterId.Guid, () => StartQueuedActionAsync(character.Value));
        }
    }

    public override IEnumerable<GameSimulation.Work> Tick()
    {
        foreach ((CharacterId characterId, Action _) in _state.Actions.Ongoing)
        {
            Maybe<Character> character = GetCharacter(characterId);
            if (!character.Success)
            {
                continue;
            }

            yield return new GameSimulation.Work(characterId.Guid, () => TickActionAsync(character.Value));
        }
    }

    public override IEnumerable<GameSimulation.Work> Late()
    {
        foreach ((CharacterId characterId, Action action) in _state.Actions.Ongoing)
        {
            Maybe<Character> character = GetCharacter(characterId);
            if (!character.Success)
            {
                _logger.LogError("Character {characterId} not found. Action will be removed without calling {method}.", characterId, nameof(Action.EndAsync));
                continue;
            }

            if (!action.IsOver(_state, character.Value))
            {
                continue;
            }

            yield return new GameSimulation.Work(characterId.Guid, () => RemoveActionAsync(character.Value));
        }
    }

    public override IEnumerable<GameSimulation.Work> PostTick()
    {
        yield return new GameSimulation.Work(default, PostTickAsync);
    }

    async Task StartQueuedActionAsync(Character character)
    {
        Maybe performed = await _state.Actions.StartQueuedActionAsync(character);
        if (!performed.Success)
        {
            _logger.LogError("Could not perform queued action for character {character}: {reason}", character, performed.WhyNot);
        }
    }

    async Task TickActionAsync(Character character)
    {
        Maybe performed = await _state.Actions.TickOngoingActionAsync(character);
        if (!performed.Success)
        {
            _logger.LogError("Could not perform ongoing action for character {character}: {reason}", character, performed.WhyNot);
        }
    }

    async Task RemoveActionAsync(Character character)
    {
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

    Maybe<Character> GetCharacter(CharacterId characterId)
    {
        Character? character = _state.Entities.Get<Character>(characterId);
        if (character == null)
        {
            return $"Character {characterId} not found";
        }

        return character;
    }
}
