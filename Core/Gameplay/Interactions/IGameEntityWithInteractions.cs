using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Gameplay.Interactions;

public interface IGameEntityWithInteractions : IGameEntity
{
    EntityInteractions Interactions { get; }
}
