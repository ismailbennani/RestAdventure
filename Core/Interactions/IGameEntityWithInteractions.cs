using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Interactions;

public interface IGameEntityWithInteractions : IGameEntity
{
    EntityInteractions Interactions { get; }
}
