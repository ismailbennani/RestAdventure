﻿using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Notifications;
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
        if (_entity.Busy)
        {
            return "Entity is busy";
        }

        bool isAccessible = state.Content.Maps.Locations.AreConnected(_entity.Location, location);
        if (!isAccessible)
        {
            return "Target location is not accessible";
        }

        return true;
    }

    public async Task<Maybe> MoveToAsync(GameState state, Location location)
    {
        Maybe canMove = CanMoveTo(state, location);
        if (!canMove)
        {
            return canMove;
        }

        Location oldLocation = _entity.Location;
        _entity.Location = location;

        await state.Publisher.Publish(new GameEntityMovedToLocation { Entity = _entity, OldLocation = oldLocation, NewLocation = _entity.Location });

        return true;
    }
}
