using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Interactions;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Gameplay.Actions;

/// <summary>
///     Set the action that a character will perform on next tick.
/// </summary>
public class GameActions
{
    readonly Dictionary<CharacterId, CharacterAction> _actions = new();
    readonly Dictionary<CharacterId, CharacterActionResult> _results = new();

    public GameActions(GameState gameState)
    {
        GameState = gameState;
    }

    internal GameState GameState { get; }

    /// <summary>
    ///     Make the character move to the given location.
    /// </summary>
    public void MoveToLocation(Character character, Location location)
    {
        AssertCharacterCanPerformAction(character);
        _actions[character.Id] = new CharacterMoveToLocationAction(character, location);
    }

    /// <summary>
    ///     Perform an interaction.
    /// </summary>
    public void Interact(Character character, Interaction interaction, IGameEntityWithInteractions entity)
    {
        AssertCharacterCanPerformAction(character);
        _actions[character.Id] = new CharacterInteractWithEntityAction(character, interaction, entity);
    }

    public void Cancel(Character character) => _actions.Remove(character.Id);

    public CharacterActionResult? GetLastActionResult(Character character) => _results.GetValueOrDefault(character.Id);

    public CharacterAction? GetNextAction(Character character) => _actions.GetValueOrDefault(character.Id);

    public void ResolveActions(GameContent content, GameState state)
    {
        _results.Clear();

        foreach ((CharacterId characterId, CharacterAction action) in _actions)
        {
            CharacterActionResolution resolution = action.Perform(content, state);
            _results[characterId] = new CharacterActionResult
            {
                Tick = state.Tick,
                CharacterId = characterId,
                Action = action,
                Success = resolution.Success,
                FailureReason = resolution.ErrorMessage
            };
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
