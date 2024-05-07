using Microsoft.Extensions.Logging;
using RestAdventure.Core.Actions.Notifications;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Actions;

/// <summary>
///     Set the action that a character will perform on next tick.
/// </summary>
public class GameActions
{
    readonly ILogger<GameActions> _logger;
    readonly Dictionary<CharacterId, CharacterAction> _actions = new();
    readonly Dictionary<CharacterId, CharacterActionResult> _results = new();

    public GameActions(GameState gameState, ILogger<GameActions> logger)
    {
        GameState = gameState;
        _logger = logger;
    }

    internal GameState GameState { get; }

    /// <summary>
    ///     Make the character move to the given location.
    /// </summary>
    public void MoveToLocation(Character character, Location location)
    {
        AssertCharacterCanPerformAction(character);
        _actions[character.Id] = new CharacterMoveToLocationAction(location);
    }

    /// <summary>
    ///     Perform an interaction.
    /// </summary>
    public void Interact(Character character, Interaction interaction, IInteractibleEntity entity)
    {
        AssertCharacterCanPerformAction(character);
        _actions[character.Id] = new CharacterInteractWithEntityAction(interaction, entity);
    }

    public void Cancel(Character character) => _actions.Remove(character.Id);

    public CharacterActionResult? GetLastActionResult(Character character) => _results.GetValueOrDefault(character.Id);

    public CharacterAction? GetNextAction(Character character) => _actions.GetValueOrDefault(character.Id);

    public async Task ResolveActionsAsync(GameState state)
    {
        _results.Clear();

        foreach ((CharacterId characterId, CharacterAction action) in _actions)
        {
            Character? character = state.Entities.Get<Character>(characterId);
            if (character == null)
            {
                _logger.LogWarning("Cannot find character {id}", characterId);
                continue;
            }

            Maybe resolution = await action.PerformAsync(state, character);
            CharacterActionResult result = new()
            {
                Tick = state.Tick,
                Action = action,
                Success = resolution.Success,
                WhyNot = resolution.WhyNot
            };
            _results[characterId] = result;

            await GameState.Publisher.Publish(new ActionPerformed { Character = character, Result = result });
        }

        _actions.Clear();
    }

    void AssertCharacterCanPerformAction(Character character)
    {
        if (GameState.Interactions.GetCharacterInteraction(character) != null)
        {
            throw new InvalidOperationException($"Character {character} cannot perform action because they are currently locked in an interaction.");
        }
    }
}
