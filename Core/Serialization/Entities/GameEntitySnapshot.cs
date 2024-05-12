using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Serialization.Entities;

public class GameEntitySnapshot
{
    protected GameEntitySnapshot(GameEntityId id)
    {
        Id = id;
    }

    public GameEntityId Id { get; }

    public required TeamSnapshot? Team { get; init; }
    public required string Name { get; init; }
    public required Location Location { get; init; }
    public required bool Busy { get; init; }

    public static GameEntitySnapshot Take(IGameEntity entity) =>
        entity switch
        {
            Character character => CharacterSnapshot.Take(character),
            MonsterGroup monsterGroup => MonsterGroupSnapshot.Take(monsterGroup),
            StaticObjectInstance staticObjectInstance => StaticObjectInstanceSnapshot.Take(staticObjectInstance),
            _ => new GameEntitySnapshot(entity.Id)
            {
                Team = entity.Team == null ? null : TeamSnapshot.Take(entity.Team),
                Name = entity.Name,
                Location = entity.Location,
                Busy = entity.Busy
            }
        };
}

public abstract class GameEntitySnapshot<TId> : GameEntitySnapshot where TId: GameEntityId
{
    protected GameEntitySnapshot(TId id) : base(id)
    {
        Id = id;
    }

    public new TId Id { get; }
}
