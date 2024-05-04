﻿using RestAdventure.Core.Characters;

namespace RestAdventure.Core.Gameplay.Actions;

public abstract class CharacterAction
{
    protected CharacterAction(Character character)
    {
        TeamId = character.Team.Id;
        CharacterId = character.Id;
    }

    protected Guid TeamId { get; }
    protected Guid CharacterId { get; }

    public abstract CharacterActionResolution Perform(GameState state);
}
