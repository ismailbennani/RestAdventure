using MediatR;
using RestAdventure.Core.Entities.Notifications;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;

namespace RestAdventure.Core.Entities;

public class GameEntities
{
    readonly Dictionary<GameEntityId, GameEntity> _entities = [];

    public GameEntities(GameState gameState)
    {
        GameState = gameState;
    }

    internal GameState GameState { get; }

    public IEnumerable<GameEntity> All => _entities.Values;

    public async Task RegisterAsync(GameEntity entity)
    {
        _entities[entity.Id] = entity;

        entity.Moved += (_, args) => PublishSync(new GameEntityMovedToLocation { Entity = entity, OldLocation = args.OldLocation, NewLocation = args.NewLocation });

        if (entity is IEntityWithInventory withInventory)
        {
            RegisterInventoryEvents(withInventory);
        }

        if (entity is IEntityWithJobs withJobs)
        {
            RegisterJobsEvents(withJobs);
        }

        await GameState.Publisher.Publish(new GameEntityCreated { Entity = entity });
        await GameState.Publisher.Publish(new GameEntityMovedToLocation { Entity = entity, OldLocation = null, NewLocation = entity.Location });
    }

    public async Task UnregisterAsync(GameEntity entity)
    {
        if (!_entities.ContainsKey(entity.Id))
        {
            throw new InvalidOperationException($"Entity {entity} is not registered");
        }

        _entities.Remove(entity.Id);

        await GameState.Publisher.Publish(new GameEntityDeleted { Entity = entity });
    }

    public GameEntity? Get(GameEntityId id) => _entities.GetValueOrDefault(id);
    public TEntity? Get<TEntity>(GameEntityId id) where TEntity: GameEntity => Get(id) as TEntity;

    void RegisterInventoryEvents(IEntityWithInventory entity) =>
        entity.Inventory.Changed += (_, args) => PublishSync(
            new GameEntityInventoryChanged
            {
                Entity = entity,
                ItemInstance = args.ItemInstance,
                OldCount = args.OldCount,
                NewCount = args.NewCount
            }
        );

    void RegisterJobsEvents(IEntityWithJobs entity)
    {
        entity.Jobs.JobLearned += (_, job) => PublishSync(new GameEntityLearnedJob { Entity = entity, Job = job });
        entity.Jobs.JobLeveldUp += (_, args) => PublishSync(new GameEntityJobLeveledUp { Entity = entity, Job = args.Job, OldLevel = args.OldLevel, NewLevel = args.NewLevel });
    }

    void PublishSync(INotification notification) => GameState.Publisher.Publish(notification).Wait();
}
