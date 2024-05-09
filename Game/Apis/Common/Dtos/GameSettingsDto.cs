using System.ComponentModel.DataAnnotations;
using RestAdventure.Core;

namespace RestAdventure.Game.Apis.Common.Dtos;

/// <summary>
///     Game settings
/// </summary>
public class GameSettingsDto
{
    /// <summary>
    ///     The max number of characters in a team
    /// </summary>
    [Required]
    public int MaxTeamSize { get; set; } = 3;
}

static class GameSettingsMappingExtensions
{
    public static GameSettingsDto ToDto(this GameSettings settings) => new() { MaxTeamSize = settings.MaxTeamSize };
}
