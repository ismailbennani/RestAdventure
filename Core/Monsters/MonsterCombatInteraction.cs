using RestAdventure.Core.Combat;
using RestAdventure.Core.Interactions;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Monsters;

public class MonsterCombatInteraction : CombatInteraction
{
    public MonsterCombatInteraction(GameCombats combats) : base(combats)
    {
    }

    protected override async Task<Maybe> CanInteractInternalAsync(IInteractingEntity source, IInteractibleEntity target)
    {
        Maybe baseResult = await base.CanInteractInternalAsync(source, target);
        if (!baseResult)
        {
            return baseResult;
        }

        if (target is not MonsterInstance)
        {
            return "Target is not a monster";
        }

        return true;
    }
}
