using RestAdventure.Core.Interactions;
using RestAdventure.Core.Monsters;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatInteraction : Interaction
{
    public PveCombatInteraction(GameCombats combats)
    {
        Combats = combats;
    }

    public override string Name => "combat";
    protected GameCombats Combats { get; }

    protected override Maybe CanInteractInternal(IInteractingEntity source, IInteractibleEntity target)
    {
        if (source is not IGameEntityWithCombatStatistics)
        {
            return "Source cannot enter combat";
        }

        if (target is not IGameEntityWithCombatStatistics)
        {
            return "Target cannot enter combat";
        }

        if (target is not MonsterInstance)
        {
            return "Target is not a monster";
        }

        return true;
    }

    protected override async Task<Maybe<InteractionInstance>> InstantiateInteractionInternalAsync(IInteractingEntity source, IInteractibleEntity target)
    {
        CombatInPreparation combatInPreparation = await Combats.StartCombatAsync((IGameEntityWithCombatStatistics)source, (IGameEntityWithCombatStatistics)target);
        return new PveCombatInteractionInstance(combatInPreparation, source, this, target);
    }
}
