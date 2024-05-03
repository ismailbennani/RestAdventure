using System.ComponentModel.DataAnnotations;

namespace RestAdventure.Game.Apis.AdminApi.Players.Requests;

public class RegisterPlayerRequestDto
{
    [Required]
    public required Guid PlayerId { get; init; }

    [Required]
    public required string PlayerName { get; init; }
}
