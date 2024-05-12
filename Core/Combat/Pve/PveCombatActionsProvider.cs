using RestAdventure.Core.Actions;
using RestAdventure.Core.Serialization;
using RestAdventure.Core.Serialization.Entities;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatActionsProvider : IActionsProvider
{
    public IEnumerable<Action> GetActions(GameSnapshot state, CharacterSnapshot character)
    {
        IEnumerable<MonsterGroupSnapshot> groups = state.Entities.Values.OfType<MonsterGroupSnapshot>().Where(group => group.Location == character.Location);
        foreach (MonsterGroupSnapshot group in groups)
        {
            if (group.OngoingCombatId != null)
            {
                yield return new JoinAndPlayPveCombatAction(group.Id, group.OngoingCombatId);
            }
            else
            {
                yield return new StartAndPlayPveCombatAction(group.Id);
            }
        }
    }
}
