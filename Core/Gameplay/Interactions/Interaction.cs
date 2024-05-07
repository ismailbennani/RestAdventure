using RestAdventure.Core.Characters;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Gameplay.Interactions;

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

    public abstract Task<Maybe> CanInteractAsync(GameState state, Character character, IGameEntityWithInteractions entity);
    public abstract Task<Maybe<InteractionInstance>> InstantiateInteractionAsync(GameState state, Character character, IGameEntityWithInteractions entity);

    public override string ToString() => Name;
}

public class CanInteractResult
{
    public required bool CanInteract { get; init; }
    public string? WhyNot { get; init; }
}
