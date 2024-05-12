using RestAdventure.Core.Actions;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Serialization;
using RestAdventure.Core.Serialization.Entities;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Maps;

public class MoveActionsProvider : IActionsProvider
{
    public IEnumerable<Action> GetActions(GameSnapshot state, CharacterSnapshot character)
    {
        IEnumerable<Location> connectedLocations = state.Content.Maps.Locations.ConnectedTo(character.Location);
        foreach (Location location in connectedLocations)
        {
            yield return new MoveAction(location);
        }
    }
}
