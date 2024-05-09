﻿using Microsoft.Extensions.Logging;
using RestAdventure.Core.Actions.Providers;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Monsters;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatActionsProvider : IActionsProvider
{
    readonly ILoggerFactory _loggerFactory;

    public PveCombatActionsProvider(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public IEnumerable<Action> GetActions(GameState state, Character character)
    {
        IEnumerable<MonsterInstance> monsters = state.Entities.AtLocation<MonsterInstance>(character.Location);
        foreach (MonsterInstance monster in monsters)
        {
            yield return new PveCombatAction([monster], _loggerFactory.CreateLogger<PveCombatAction>());
        }
    }
}