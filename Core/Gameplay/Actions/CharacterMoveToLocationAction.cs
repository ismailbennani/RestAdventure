using RestAdventure.Core.Characters;
using RestAdventure.Core.Maps;

namespace RestAdventure.Core.Gameplay.Actions;

public class CharacterMoveToLocationAction : CharacterAction
{
    public CharacterMoveToLocationAction(Character character, MapLocation location) : base(character)
    {
        LocationId = location.Id;
    }

    public MapLocationId LocationId { get; }

    public override CharacterActionResolution Perform(GameContent content, GameState state)
    {
        Character character = state.Characters.RequireCharacter(TeamId, CharacterId);
        MapLocation location = content.Maps.RequireLocation(LocationId);

        bool isAccessible = content.Maps.IsConnected(character.Location, location);
        if (!isAccessible)
        {
            return new CharacterActionResolution { Success = false, ErrorMessage = $"Map {location.Id} is inaccessible" };
        }

        character.Location = location;
        return new CharacterActionResolution { Success = true };
    }
}
