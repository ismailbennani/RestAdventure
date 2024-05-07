using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Interactions;

public interface IInteractibleEntity : IGameEntity
{
    public bool Disabled { get; }
    void Enable();
    void Disable();
}
