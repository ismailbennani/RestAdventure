using RestAdventure.Core.History.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Characters;

/// <summary>
///     Character deleted history entry
/// </summary>
public class CharacterDeletedHistoryEntryDto : CharacterHistoryEntryDto
{
}

static class CharacterDeletedHistoryEntryMappingExtensions
{
    public static CharacterDeletedHistoryEntryDto ToDto(this EntityDeletedHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick
        };
}
