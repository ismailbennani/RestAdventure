﻿using MediatR;
using RestAdventure.Core.Items;

namespace RestAdventure.Core.Entities.Notifications;

public class EntityInventoryChanged : INotification
{
    public required IEntityWithInventory Entity { get; init; }
    public required ItemInstance ItemInstance { get; init; }
    public required int OldCount { get; init; }
    public required int NewCount { get; init; }

    public override string ToString() => $"{Entity}[{ItemInstance}: {OldCount} -> {NewCount}]";
}