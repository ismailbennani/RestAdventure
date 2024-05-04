using System.ComponentModel.DataAnnotations;

namespace RestAdventure.Game.Apis.AdminApi.Controllers.Players.Requests;

public class RegisterPlayerRequestDto
{
    [Required]
    public required Guid PlayerGuid { get; init; }

    [Required]
    public required string PlayerName { get; init; }
}
