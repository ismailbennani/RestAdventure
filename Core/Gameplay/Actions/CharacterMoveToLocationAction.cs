using RestAdventure.Core.Characters;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Gameplay.Actions;

public class CharacterMoveToLocationAction : CharacterAction
{
    public CharacterMoveToLocationAction(Character character, Location location) : base(character)
    {
        Location = location;
    }

    public Location Location { get; }

    public override CharacterActionResolution Perform(GameContent content, GameState state)
    {
        bool isAccessible = content.Maps.Locations.AreConnected(Character.Location, Location);
        if (!isAccessible)
        {
            return new CharacterActionResolution { Success = false, ErrorMessage = $"Map {Location.Id} is inaccessible" };
        }

        Character.MoveTo(Location);

        return new CharacterActionResolution { Success = true };
    }
}
