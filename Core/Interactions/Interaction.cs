using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Interactions;

public abstract class Interaction
{
    /// <summary>
    ///     The unique name of the interaction
    /// </summary>
    public abstract string Name { get; }

    public Maybe CanInteract(IInteractingEntity source, IInteractibleEntity target)
    {
        if (source is IInteractibleEntity { Disabled: true })
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

        return CanInteractInternal(source, target);
    }

    protected abstract Maybe CanInteractInternal(IInteractingEntity source, IInteractibleEntity target);

    public async Task<Maybe<InteractionInstance>> InstantiateInteractionAsync(IInteractingEntity source, IInteractibleEntity target)
    {
        Maybe canInteract = CanInteract(source, target);
        if (!canInteract.Success)
        {
            return canInteract.WhyNot;
        }

        return await InstantiateInteractionInternalAsync(source, target);
    }

    protected abstract Task<Maybe<InteractionInstance>> InstantiateInteractionInternalAsync(IInteractingEntity source, IInteractibleEntity target);

    public override string ToString() => Name;
}
