using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Common;

/// <summary>
///     Combat entity in history entry
/// </summary>
public class CombatEntityInHistoryEntryDto
{
    /// <summary>
    ///     The unique ID of the entity
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the entity
    /// </summary>
    [Required]
    public required string Name { get; init; }
}

static class CombatEntityInHistoryEntryMappingExtensions
{
    public static CombatEntityInHistoryEntryDto ToDto(this (GameEntityId Id, string Name) entity) =>
        new()
        {
            Id = entity.Id.Guid,
            Name = entity.Name
        };
}
