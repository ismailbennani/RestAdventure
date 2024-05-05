using MediatR;
using RestAdventure.Core.Items;

namespace RestAdventure.Core.Characters.Notifications;

public class CharacterInventoryChanged : INotification
{
    public required Character Character { get; init; }
    public required ItemInstance ItemInstance { get; init; }
    public required int OldCount { get; init; }
    public required int NewCount { get; init; }

    public override string ToString() => $"{Character}[{ItemInstance}: {OldCount} -> {NewCount}]";
}
