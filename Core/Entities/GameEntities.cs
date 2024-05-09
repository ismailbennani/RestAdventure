using MediatR;
using RestAdventure.Core.Entities.Notifications;
using RestAdventure.Core.Extensions;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Jobs.Notifications;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Entities;

public class GameEntities : IDisposable
{
    readonly IPublisher _publisher;
    readonly Dictionary<GameEntityId, IGameEntity> _entities = [];

    public GameEntities(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public IEnumerable<IGameEntity> All => _entities.Values;

    public async Task AddAsync(IGameEntity entity)
    {
        _entities[entity.Id] = entity;

        if (entity is IGameEntityWithInventory withInventory)
        {
            RegisterInventoryEvents(withInventory);
        }

        if (entity is IGameEntityWithJobs withJobs)
        {
            RegisterJobsEvents(withJobs);
        }

        await _publisher.Publish(new GameEntityCreated { Entity = entity });
        await _publisher.Publish(new GameEntityMovedToLocation { Entity = entity, OldLocation = null, NewLocation = entity.Location });
    }

    public async Task DestroyAsync(IGameEntity entity)
    {
        if (!_entities.ContainsKey(entity.Id))
        {
            throw new InvalidOperationException($"Entity {entity} is not registered");
        }

        _entities.Remove(entity.Id);

        await _publisher.Publish(new GameEntityDeleted { Entity = entity });

        entity.Dispose();
    }

    public IGameEntity? Get(GameEntityId id) => _entities.SingleOrDefault(kv => kv.Key.Guid == id.Guid).Value;
    public TEntity? Get<TEntity>(GameEntityId id) where TEntity: class, IGameEntity => Get(id) as TEntity;
    public IEnumerable<IGameEntity> AtLocation(Location location) => All.Where(e => e.Location == location);
    public IEnumerable<TEntity> AtLocation<TEntity>(Location location) where TEntity: IGameEntity => All.OfType<TEntity>().Where(e => e.Location == location);

    void RegisterInventoryEvents(IGameEntityWithInventory entity) =>
        entity.Inventory.Changed += (_, args) => _publisher.PublishSync(
            new GameEntityInventoryChanged
            {
                Entity = entity,
                ItemInstance = args.ItemInstance,
                OldCount = args.OldCount,
                NewCount = args.NewCount
            }
        );

    void RegisterJobsEvents(IGameEntityWithJobs entity)
    {
        entity.Jobs.JobLearned += (_, job) => _publisher.PublishSync(new GameEntityLearnedJob { Entity = entity, Job = job });
        entity.Jobs.JobGainedExperience += (_, args) => _publisher.PublishSync(
            new GameEntityJobGainedExperience { Entity = entity, Job = args.Job, OldExperience = args.OldExperience, NewExperience = args.NewExperience }
        );
        entity.Jobs.JobLeveledUp += (_, args) =>
            _publisher.PublishSync(new GameEntityJobLeveledUp { Entity = entity, Job = args.Job, OldLevel = args.OldLevel, NewLevel = args.NewLevel });
    }


    public void Dispose()
    {
        foreach (IGameEntity entity in _entities.Values)
        {
            entity.Dispose();
        }
        _entities.Clear();
        GC.SuppressFinalize(this);
    }
}
