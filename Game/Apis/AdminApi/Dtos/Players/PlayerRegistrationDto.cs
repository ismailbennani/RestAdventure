using System.ComponentModel.DataAnnotations;
using RestAdventure.Game.Registration;

namespace RestAdventure.Game.Apis.AdminApi.Dtos.Players;

public class PlayerRegistrationDto
{
    /// <summary>
    ///     The API key that grants access to the Game API
    /// </summary>
    [Required]
    public required Guid ApiKey { get; init; }

    /// <summary>
    ///     The player that is associated with the registration
    /// </summary>
    [Required]
    public required PlayerIdentityDto Player { get; init; }

    /// <summary>
    ///     The date at which the registration has been created
    /// </summary>
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
