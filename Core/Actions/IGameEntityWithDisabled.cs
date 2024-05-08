using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Actions;

public interface IGameEntityWithDisabled : IGameEntity
{
    public bool Disabled { get; }
}
