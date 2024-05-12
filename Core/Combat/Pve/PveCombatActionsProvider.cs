using RestAdventure.Core.Actions;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatActionsProvider : IActionsProvider
{
    public IEnumerable<Action> GetActions(Game state, Character character)
    {
        IEnumerable<MonsterGroup> groups = state.Entities.AtLocation<MonsterGroup>(character.Location);
        foreach (MonsterGroup group in groups)
        {
            if (group.JoinCombatAction != null)
            {
                yield return group.JoinCombatAction;
            }
            else
            {
                yield return group.StartCombatAction;
            }
        }
    }
}
