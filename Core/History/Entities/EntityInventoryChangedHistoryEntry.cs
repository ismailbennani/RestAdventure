using MediatR;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Notifications;
using RestAdventure.Core.Items;

namespace RestAdventure.Core.History.Entities;

public class EntityInventoryChangedHistoryEntry : EntityHistoryEntry
{
    public EntityInventoryChangedHistoryEntry(IGameEntity entity, ItemInstance itemInstance, int oldCount, int newCount, long tick) : base(entity, tick)
    {
        ItemInstanceId = itemInstance.Id;
        ItemId = itemInstance.Item.Id;
        ItemName = itemInstance.Item.Name;
        OldCount = oldCount;
        NewCount = newCount;
    }

    public ItemInstanceId ItemInstanceId { get; }
    public ItemId ItemId { get; }
    public string ItemName { get; }
    public int OldCount { get; }
    public int NewCount { get; }
}

public class CreateEntityInventoryChangedHistoryEntry : INotificationHandler<GameEntityInventoryChanged>
{
    readonly GameService _gameService;

    public CreateEntityInventoryChangedHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(GameEntityInventoryChanged notification, CancellationToken cancellationToken)
    {
        Game state = _gameService.RequireGame();
        EntityInventoryChangedHistoryEntry entry = new(notification.Entity, notification.ItemInstance, notification.OldCount, notification.NewCount, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
