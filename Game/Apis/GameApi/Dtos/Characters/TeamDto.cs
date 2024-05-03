using RestAdventure.Core.Characters;

namespace RestAdventure.Game.Apis.GameApi.Dtos.Characters;

/// <summary>
///     Team of characters
/// </summary>
public class TeamDto
{
    /// <summary>
    ///     The characters in the team
    /// </summary>
    public required IReadOnlyCollection<TeamCharacterDto> Characters { get; init; }
}

static class TeamMappingExtensions
{
    public static TeamDto ToDto(this TeamDbo team) => new() { Characters = team.Characters.Select(c => c.ToDto()).ToArray() };
}
