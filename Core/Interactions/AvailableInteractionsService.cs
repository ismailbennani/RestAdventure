using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Interactions.Providers;

namespace RestAdventure.Core.Interactions;

public class AvailableInteractionsService
{
    readonly IReadOnlyCollection<IInteractionProvider> _providers;

    public AvailableInteractionsService(IReadOnlyCollection<IInteractionProvider> providers)
    {
        _providers = providers;
    }

    public IEnumerable<Interaction> GetAvailableInteractions(Character character, IGameEntity entity) => _providers.SelectMany(p => p.GetAvailableInteractions(character, entity));
}
