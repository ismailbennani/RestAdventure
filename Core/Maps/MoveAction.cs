using RestAdventure.Core.Characters;
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
    public override bool IsOver(GameState state, Character character) => true;
    protected override Task<Maybe> OnStartAsync(GameState state, Character character) => Task.FromResult(character.Movement.MoveTo(state, Location));
}
