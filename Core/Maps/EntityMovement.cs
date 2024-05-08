using RestAdventure.Core.Entities;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Maps;

public class EntityMovement
{
    readonly GameEntity _entity;

    public EntityMovement(GameEntity entity)
    {
        _entity = entity;
    }

    public Maybe CanMoveTo(GameState state, Location location)
    {
        bool isAccessible = state.Content.Maps.Locations.AreConnected(_entity.Location, location);
        if (!isAccessible)
        {
            return "Target location is not accessible";
        }

        return true;
    }

    public Maybe MoveTo(GameState state, Location location)
    {
        Maybe canMove = CanMoveTo(state, location);
        if (!canMove)
        {
            return canMove;
        }

        _entity.SetLocation(location);
        return true;
    }
}
