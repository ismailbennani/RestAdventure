using RestAdventure.Core.Actions;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatActionsProvider : IActionsProvider
{
    public IEnumerable<Action> GetActions(GameState state, Character character)
    {
        IEnumerable<IGrouping<Team?, MonsterInstance>> monsters = state.Entities.AtLocation<MonsterInstance>(character.Location).GroupBy(m => m.Team);
        foreach (IGrouping<Team?, MonsterInstance> group in monsters)
        {
            yield return new PveCombatAction(group.ToArray());
        }
    }
}
