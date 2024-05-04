using RestAdventure.Core.Characters;
using RestAdventure.Core.Maps;

namespace RestAdventure.Core.Gameplay.Actions;

/// <summary>
///     Set the action that a character will perform on next tick.
/// </summary>
public class CharacterActionsService
{
    readonly Dictionary<Guid, CharacterAction> _actions = new();
    readonly Dictionary<Guid, CharacterActionResult> _results = new();

    /// <summary>
    ///     Make the character move to the given location on next tick.
    /// </summary>
    public void MoveToLocation(Character character, MapLocation location) => _actions[character.Id] = new CharacterMoveToLocationAction(character, location);

    public CharacterActionResult? GetLastActionResult(Character character) => _results.GetValueOrDefault(character.Id);

    public CharacterAction? GetNextAction(Character character) => _actions.GetValueOrDefault(character.Id);

    public void ResolveActions(GameState state)
    {
        _results.Clear();

        foreach ((Guid characterId, CharacterAction action) in _actions)
        {
            CharacterActionResolution resolution = action.Perform(state);
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
}

public class CharacterActionResult
{
    public required long Tick { get; init; }
    public required Guid CharacterId { get; init; }
    public required CharacterAction Action { get; init; }
    public required bool Success { get; init; }
    public string? FailureReason { get; init; }
}
