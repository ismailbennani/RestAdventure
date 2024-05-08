using RestAdventure.Core.Actions;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Maps;

public class CharacterMoveToLocationAction : CharacterAction
{
    public CharacterMoveToLocationAction(Location location)
    {
        Location = location;
    }

    public Location Location { get; }

    public override Task<Maybe> PerformAsync(GameState state, Character character) => Task.FromResult(character.Movement.MoveTo(state, Location));
}
