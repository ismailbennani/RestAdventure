using RestAdventure.Core.Characters;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Gameplay.Actions;

public class CharacterMoveToLocationAction : CharacterAction
{
    public CharacterMoveToLocationAction(Location location)
    {
        Location = location;
    }

    public Location Location { get; }

    public override CharacterActionResolution Perform(GameContent content, GameState state, Character character)
    {
        bool isAccessible = content.Maps.Locations.AreConnected(character.Location, Location);
        if (!isAccessible)
        {
            return new CharacterActionResolution { Success = false, ErrorMessage = $"Map {Location.Id} is inaccessible" };
        }

        character.MoveTo(Location);

        return new CharacterActionResolution { Success = true };
    }
}
