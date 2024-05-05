using MediatR;
using RestAdventure.Core.Entities.Notifications;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;

namespace RestAdventure.Core.Entities;

public class GameEntities
{
    readonly Dictionary<EntityId, Entity> _entities = [];

    public GameEntities(GameState gameState)
    {
        GameState = gameState;
    }

    internal GameState GameState { get; }

    public IEnumerable<Entity> All => _entities.Values;

    public async Task RegisterAsync(Entity entity)
    {
        _entities[entity.Id] = entity;

        entity.Moved += (_, args) => PublishSync(new EntityMovedToLocation { Entity = entity, OldLocation = args.OldLocation, NewLocation = args.NewLocation });

        if (entity is IEntityWithInventory withInventory)
        {
            RegisterInventoryEvents(withInventory);
        }

        if (entity is IEntityWithJobs withJobs)
        {
            RegisterJobsEvents(withJobs);
        }

        await GameState.Publisher.Publish(new EntityCreated { Entity = entity });
        await GameState.Publisher.Publish(new EntityMovedToLocation { Entity = entity, OldLocation = null, NewLocation = entity.Location });
    }

    public async Task UnregisterAsync(Entity entity)
    {
        if (!_entities.ContainsKey(entity.Id))
        {
            throw new InvalidOperationException($"Entity {entity} is not registered");
        }

        _entities.Remove(entity.Id);

        await GameState.Publisher.Publish(new EntityDeleted { Entity = entity });
    }

    public Entity? Get(EntityId id) => _entities.GetValueOrDefault(id);
    public TEntity? Get<TEntity>(EntityId id) where TEntity: Entity => Get(id) as TEntity;

    void RegisterInventoryEvents(IEntityWithInventory entity) =>
        entity.Inventory.Changed += (_, args) => PublishSync(
            new EntityInventoryChanged
            {
                Entity = entity,
                ItemInstance = args.ItemInstance,
                OldCount = args.OldCount,
                NewCount = args.NewCount
            }
        );

    void RegisterJobsEvents(IEntityWithJobs entity)
    {
        entity.Jobs.JobLearned += (_, job) => PublishSync(new EntityLearnedJob { Entity = entity, Job = job });
        entity.Jobs.JobLeveldUp += (_, args) => PublishSync(new EntityJobLeveledUp { Entity = entity, Job = args.Job, OldLevel = args.OldLevel, NewLevel = args.NewLevel });
    }

    void PublishSync(INotification notification) => GameState.Publisher.Publish(notification).Wait();
}
