using RestAdventure.Core.Characters;
using RestAdventure.Core.Maps;
using Xtensive.Orm;

namespace RestAdventure.Core.Gameplay.Actions;

public abstract class CharacterAction
{
    protected CharacterAction(CharacterDbo character)
    {
        Character = character;
    }

    protected Ref<CharacterDbo> Character { get; }

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
        Location = location;
    }

    public Ref<MapLocationDbo> Location { get; }


    public override async Task<CharacterActionResolution> PerformAsync()
    {
        bool isAccessible = await Character.Value.Location.ConnectedLocations.ContainsAsync(Location.Value);
        if (!isAccessible)
        {
            return new CharacterActionResolution { Success = false, ErrorMessage = $"Map {Location.Value.Id} is inaccessible" };
        }

        Character.Value.Location = Location.Value;
        return new CharacterActionResolution { Success = true };
    }
}
