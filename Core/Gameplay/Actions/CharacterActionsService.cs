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
    public void MoveToLocation(CharacterDbo character, MapLocationDbo location) => _actions[character.Id] = new CharacterMoveToLocationAction(character, location);

    public CharacterActionResult? GetLastActionResult(CharacterDbo character) => _results.GetValueOrDefault(character.Id);

    public CharacterAction? GetNextAction(CharacterDbo character) => _actions.GetValueOrDefault(character.Id);

    public async Task ResolveActionsAsync(GameStateDbo state)
    {
        _results.Clear();

        foreach ((Guid characterId, CharacterAction action) in _actions)
        {
            CharacterActionResolution resolution = await action.PerformAsync();
            _results[characterId] = new CharacterActionResult
            {
                Tick = state.Tick,
                CharacterId = characterId,
                Action = action,
                Success = resolution.Success,
                ErrorMessage = resolution.ErrorMessage
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
    public string? ErrorMessage { get; init; }
}
