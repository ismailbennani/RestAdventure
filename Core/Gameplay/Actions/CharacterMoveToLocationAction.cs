﻿using RestAdventure.Core.Characters;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Gameplay.Actions;

public class CharacterMoveToLocationAction : CharacterAction
{
    public CharacterMoveToLocationAction(Character character, Location location) : base(character)
    {
        LocationId = location.Id;
    }

    public LocationId LocationId { get; }

    public override CharacterActionResolution Perform(GameContent content, GameState state)
    {
        Character character = state.Characters.RequireCharacter(TeamId, CharacterId);
        Location location = content.Maps.Locations.Require(LocationId);

        bool isAccessible = content.Maps.Locations.AreConnected(character.Location, location);
        if (!isAccessible)
        {
            return new CharacterActionResolution { Success = false, ErrorMessage = $"Map {location.Id} is inaccessible" };
        }

        character.MoveToAsync(location);

        return new CharacterActionResolution { Success = true };
    }
}
