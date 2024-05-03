﻿using RestAdventure.Core.Characters;

namespace RestAdventure.Game.Apis.GameApi.Characters.Dtos;

/// <summary>
///     Team of characters
/// </summary>
public class TeamDto
{
    /// <summary>
    ///     The characters in the team
    /// </summary>
    public required IReadOnlyCollection<CharacterDto> Characters { get; init; }
}

static class TeamMappingExtensions
{
    public static TeamDto ToDto(this TeamDbo team) => new() { Characters = team.Characters.Select(c => c.ToDto()).ToArray() };
}
