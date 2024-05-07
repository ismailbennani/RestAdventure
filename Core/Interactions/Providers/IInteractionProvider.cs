using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Interactions.Providers;

public interface IInteractionProvider
{
    IEnumerable<Interaction> GetAvailableInteractions(Character character, IGameEntity entity);
}
