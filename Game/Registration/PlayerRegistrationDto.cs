using System.ComponentModel.DataAnnotations;

namespace RestAdventure.Game.Registration;

public class PlayerRegistrationDto
{
    [Required]
    public required Guid ApiKey { get; init; }

    [Required]
    public required PlayerIdentityDto Player { get; init; }

    [Required]
    public required DateTime CreationDate { get; init; }
}

static class PlayerRegistrationMappingExtensions
{
    public static PlayerRegistrationDto ToDto(this PlayerRegistrationDbo registration) =>
        new()
        {
            ApiKey = registration.ApiKey,
            Player = registration.Player.ToDto(),
            CreationDate = registration.CreationDate
        };
}
