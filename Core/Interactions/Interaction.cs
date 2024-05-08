using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Interactions;

public abstract class Interaction
{
    /// <summary>
    ///     The unique name of the interaction
    /// </summary>
    public abstract string Name { get; }

    public async Task<Maybe> CanInteractAsync(IInteractingEntity source, IInteractibleEntity target)
    {
        if (source.CurrentInteraction != null)
        {
            return "Character is busy";
        }

        if (target.Disabled)
        {
            return "Target is busy";
        }

        if (source.Location != target.Location)
        {
            return "Target is inaccessible";
        }

        return await CanInteractInternalAsync(source, target);
    }

    protected abstract Task<Maybe> CanInteractInternalAsync(IInteractingEntity source, IInteractibleEntity target);

    public async Task<Maybe<InteractionInstance>> InstantiateInteractionAsync(IInteractingEntity source, IInteractibleEntity target)
    {
        Maybe canInteract = await CanInteractAsync(source, target);
        if (!canInteract.Success)
        {
            return canInteract.WhyNot;
        }

        return await InstantiateInteractionInternalAsync(source, target);
    }

    protected abstract Task<Maybe<InteractionInstance>> InstantiateInteractionInternalAsync(IInteractingEntity source, IInteractibleEntity target);

    public override string ToString() => Name;
}
