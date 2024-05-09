using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Kernel.Errors;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Maps;

public class MoveAction : Action
{
    public MoveAction(Location location) : base("move")
    {
        Location = location;
    }

    public Location Location { get; }

    protected override Maybe CanPerformInternal(GameState state, Character character) => character.Movement.CanMoveTo(state, Location);

    protected override async Task<Maybe> OnStartAsync(GameState state, Character character)
    {
        Over = true;
        return await character.Movement.MoveToAsync(state, Location);
    }
}
