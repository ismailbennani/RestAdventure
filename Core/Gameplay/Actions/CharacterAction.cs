using RestAdventure.Core.Characters;
using RestAdventure.Core.Maps;
using Xtensive.Orm;

namespace RestAdventure.Core.Gameplay.Actions;

public abstract class CharacterAction
{
    protected CharacterAction(CharacterDbo character)
    {
        CharacterId = character.Id;
    }

    protected Guid CharacterId { get; }

    public abstract Task<CharacterActionResolution> PerformAsync();
}

public class CharacterActionResolution
{
    public required bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public class CharacterMoveToLocationAction : CharacterAction
{
    public CharacterMoveToLocationAction(CharacterDbo character, MapLocationDbo location) : base(character)
    {
        LocationId = location.Id;
    }

    public Guid LocationId { get; }


    public override async Task<CharacterActionResolution> PerformAsync()
    {
        CharacterDbo? character = await Query.All<CharacterDbo>().SingleOrDefaultAsync(c => c.Id == CharacterId);
        MapLocationDbo? location = await Query.All<MapLocationDbo>().SingleOrDefaultAsync(l => l.Id == LocationId);

        bool isAccessible = await character.Location.ConnectedLocations.ContainsAsync(location);
        if (!isAccessible)
        {
            return new CharacterActionResolution { Success = false, ErrorMessage = $"Map {location.Id} is inaccessible" };
        }

        character.Location = location;
        return new CharacterActionResolution { Success = true };
    }
}
