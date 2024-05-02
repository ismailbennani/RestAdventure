namespace RestAdventure.Game.Registration;

public class PlayerRegistrationDto
{
    public required Guid ApiKey { get; init; }
    public required DateTime CreationDate { get; init; }
}

static class PlayerRegistrationMappingExtensions
{
    public static PlayerRegistrationDto ToDto(this PlayerRegistrationDbo registration) =>
        new()
        {
            ApiKey = registration.ApiKey,
            CreationDate = registration.CreationDate
        };
}
