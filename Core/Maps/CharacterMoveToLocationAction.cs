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

    public override Task<Maybe> PerformAsync(GameState state, Character character)
    {
        bool isAccessible = state.Content.Maps.Locations.AreConnected(character.Location, Location);
        if (!isAccessible)
        {
            return Task.FromResult<Maybe>($"Map {Location.Id} is inaccessible");
        }

        character.MoveTo(Location);

        return Task.FromResult<Maybe>(true);
    }
}
