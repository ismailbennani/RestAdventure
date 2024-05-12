using System.Collections;
using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Players;

public class PlayerKnowledge : IReadOnlyCollection<ResourceId>, IDisposable
{
    readonly HashSet<ResourceId> _discoveredResources = [];

    public event EventHandler<GameResource>? Discovered;

    public int Count => _discoveredResources.Count;
    public IEnumerator<ResourceId> GetEnumerator() => _discoveredResources.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_discoveredResources).GetEnumerator();

    public void Discover(GameResource resource)
    {
        if (!_discoveredResources.Add(resource.Id))
        {
            return;
        }

        Discovered?.Invoke(this, resource);
    }

    public bool Knows(GameResource resource) => _discoveredResources.Contains(resource.Id);

    public void Dispose()
    {
        Discovered = null;
        GC.SuppressFinalize(this);
    }
}
