﻿using RestAdventure.Core.Entities.Characters;

namespace RestAdventure.Core.Actions.Providers;

public interface IActionsProvider
{
    IEnumerable<Action> GetActions(GameState state, Character character);
}
