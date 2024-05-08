using RestAdventure.Core.Interactions;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Combat;

public abstract class CombatInteraction : Interaction
{
    protected CombatInteraction(GameCombats combats)
    {
        Combats = combats;
    }

    public override string Name => "combat";
    protected GameCombats Combats { get; }

    protected override Task<Maybe> CanInteractInternalAsync(IInteractingEntity source, IInteractibleEntity target)
    {
        if (source is not IGameEntityWithCombatStatistics)
        {
            return Task.FromResult<Maybe>("Source cannot enter combat");
        }

        if (target is not IGameEntityWithCombatStatistics)
        {
            return Task.FromResult<Maybe>("Target cannot enter combat");
        }

        return Task.FromResult<Maybe>(true);
    }

    protected override async Task<Maybe<InteractionInstance>> InstantiateInteractionInternalAsync(IInteractingEntity source, IInteractibleEntity target)
    {
        CombatInPreparation combatInPreparation = await Combats.StartCombatAsync((IGameEntityWithCombatStatistics)source, (IGameEntityWithCombatStatistics)target);
        return new CharacterCombatInteractionInstance(combatInPreparation, source, this, target);
    }
}
