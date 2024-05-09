using RestAdventure.Core.Actions;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatActionsProvider : IActionsProvider
{
    public IEnumerable<Action> GetActions(GameState state, Character character)
    {
        IEnumerable<MonsterInstance> monsters = state.Entities.AtLocation<MonsterInstance>(character.Location);
        foreach (MonsterInstance monster in monsters)
        {
            yield return new PveCombatAction([monster]);
        }
    }
}
