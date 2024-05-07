using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Interactions;

public record InteractionId(Guid Guid);

public abstract class Interaction
{
    /// <summary>
    ///     The unique name of the interaction
    /// </summary>
    public abstract string Name { get; }

    public abstract Task<Maybe> CanInteractAsync(GameState state, Character character, IGameEntity entity);
    public abstract Task<Maybe<InteractionInstance>> InstantiateInteractionAsync(GameState state, Character character, IGameEntity entity);

    public override string ToString() => Name;
}
