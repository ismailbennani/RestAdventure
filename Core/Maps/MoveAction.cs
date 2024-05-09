using RestAdventure.Core.Actions;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Maps;

public class MoveAction : ImmediateAction
{
    public MoveAction(Location location) : base("move")
    {
        Location = location;
    }

    public Location Location { get; }

    protected override Maybe CanPerformInternal(GameState state, Character character) => character.Movement.CanMoveTo(state, Location);
    protected override async Task PerformAsync(GameState state, Character character) => await character.Movement.MoveToAsync(state, Location);

    public override string ToString() => $"Move to ${Location}";
}
