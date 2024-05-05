using RestAdventure.Core.Characters;

namespace RestAdventure.Core.Interactions;

public record InteractionId(Guid Guid);

public abstract class Interaction
{
    /// <summary>
    ///     The unique ID of the interaction
    /// </summary>
    public InteractionId Id { get; } = new(Guid.NewGuid());

    /// <summary>
    ///     The name of the interaction
    /// </summary>
    public abstract string Name { get; }

    public abstract bool CanInteract(Character character, IEntityWithInteractions entity);
    public abstract Task<InteractionInstance> InstantiateAsync(Character character, IEntityWithInteractions entity);
}
