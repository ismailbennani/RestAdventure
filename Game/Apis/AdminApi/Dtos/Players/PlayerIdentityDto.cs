using System.ComponentModel.DataAnnotations;
using RestAdventure.Game.Registration;

namespace RestAdventure.Game.Apis.AdminApi.Dtos.Players;

public class PlayerIdentityDto
{
    /// <summary>
    ///     The unique ID of the player
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the player
    /// </summary>
    [Required]
    public required string Name { get; init; }
}

static class PlayerIdentityMappingExtensions
{
    public static PlayerIdentityDto ToDto(this PlayerIdentityDbo identity) => new() { Id = identity.Id, Name = identity.Name };
}
