using System.Collections;

namespace RestAdventure.Core.Resources;

public class GameResourcesStore<TResourceId, TResource> : IReadOnlyCollection<TResource> where TResourceId: ResourceId where TResource: GameResource<TResourceId>
{
    protected Dictionary<TResourceId, TResource> Resources { get; } = [];

    public int Count => Resources.Count;
    public IEnumerator<TResource> GetEnumerator() => Resources.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Resources).GetEnumerator();

    public void Register(TResource resource) => Resources[resource.Id] = resource;

    public void Register(IEnumerable<TResource> resources)
    {
        foreach (TResource resource in resources)
        {
            Resources[resource.Id] = resource;
        }
    }

    public TResource? Get(TResourceId resourceId) => Resources.GetValueOrDefault(resourceId);
}

public static class GameResourcesStoreExtensions
{
    public static TResource Require<TResourceId, TResource>(this GameResourcesStore<TResourceId, TResource> gameResourcesStore, TResourceId resourceId)
        where TResourceId: ResourceId where TResource: GameResource<TResourceId> =>
        gameResourcesStore.Get(resourceId) ?? throw new InvalidOperationException($"Could not find resource {resourceId}");
}
