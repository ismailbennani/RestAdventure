﻿using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Players;

public class PlayerKnowledge
{
    readonly HashSet<ResourceId> _discoveredResources = [];

    public event EventHandler<GameResource>? Discovered;

    public void Discover(GameResource resource)
    {
        if (!_discoveredResources.Add(resource.Id))
        {
            return;
        }

        Discovered?.Invoke(this, resource);
    }

    public bool Knows(GameResource resource) => _discoveredResources.Contains(resource.Id);
}
