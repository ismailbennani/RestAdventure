using System.Collections;

namespace RestAdventure.Core.Resources;

public class GameResourcesStore<TResourceId, TResource> : IReadOnlyCollection<TResource> where TResourceId: ResourceId where TResource: GameResource<TResourceId>
{
    readonly Dictionary<TResourceId, TResource> _resources = [];

    public int Count => _resources.Count;
    public IEnumerator<TResource> GetEnumerator() => _resources.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_resources).GetEnumerator();

    public void Register(TResource resource) => _resources[resource.Id] = resource;

    public void Register(IEnumerable<TResource> resources)
    {
        foreach (TResource resource in resources)
        {
            _resources[resource.Id] = resource;
        }
    }

    public TResource? Get(TResourceId resourceId) => _resources.GetValueOrDefault(resourceId);
}

public static class GameResourcesStoreExtensions
{
    public static TResource Require<TResourceId, TResource>(this GameResourcesStore<TResourceId, TResource> gameResourcesStore, TResourceId resourceId)
        where TResourceId: ResourceId where TResource: GameResource<TResourceId> =>
        gameResourcesStore.Get(resourceId) ?? throw new InvalidOperationException($"Could not find resource {resourceId}");
}
