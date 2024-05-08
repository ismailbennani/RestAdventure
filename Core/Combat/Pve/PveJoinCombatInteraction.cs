using RestAdventure.Core.Interactions;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Combat.Pve;

public class PveJoinCombatInteraction : Interaction
{
    public PveJoinCombatInteraction(CombatInPreparation combat, CombatSide side)
    {
        Combat = combat;
        Side = side;
    }

    public override string Name => "combat-join";
    CombatInPreparation Combat { get; }
    CombatSide Side { get; }

    protected override Maybe CanInteractInternal(IInteractingEntity source, IInteractibleEntity target)
    {
        if (source is not IGameEntityWithCombatStatistics)
        {
            return "Source cannot enter combat";
        }

        return true;
    }

    protected override async Task<Maybe<InteractionInstance>> InstantiateInteractionInternalAsync(IInteractingEntity source, IInteractibleEntity target)
    {
        CombatFormationInPreparation team = Combat.GetTeam(Side);

        Maybe added = team.Add((IGameEntityWithCombatStatistics)source);
        if (!added.Success)
        {
            return added.WhyNot;
        }

        return new PveCombatInteractionInstance(Combat, source, this, target);
    }
}
