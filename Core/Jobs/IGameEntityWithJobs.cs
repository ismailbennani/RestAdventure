using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Jobs;

public interface IGameEntityWithJobs : IGameEntity
{
    EntityJobs Jobs { get; }
}
