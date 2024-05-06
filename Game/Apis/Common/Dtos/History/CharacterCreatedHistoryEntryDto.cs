using RestAdventure.Core.History.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.History;

/// <summary>
///     Character created history entry
/// </summary>
public class CharacterCreatedHistoryEntryDto : CharacterHistoryEntryDto
{
}

static class CharacterCreatedHistoryEntryMappingExtensions
{
    public static CharacterCreatedHistoryEntryDto ToDto(this EntityCreatedHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick
        };
}
