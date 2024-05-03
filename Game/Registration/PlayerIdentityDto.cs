using System.ComponentModel.DataAnnotations;

namespace RestAdventure.Game.Registration;

public class PlayerIdentityDto
{
    [Required]
    public required Guid Id { get; init; }

    [Required]
    public required string Name { get; init; }
}

static class PlayerIdentityMappingExtensions
{
    public static PlayerIdentityDto ToDto(this PlayerIdentityDbo identity) => new() { Id = identity.Id, Name = identity.Name };
}
