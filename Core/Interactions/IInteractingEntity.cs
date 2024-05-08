using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Interactions;

public interface IInteractingEntity : IGameEntity
{
    InteractionInstance? CurrentInteraction { get; set; }
}
