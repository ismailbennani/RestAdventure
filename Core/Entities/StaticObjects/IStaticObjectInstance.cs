namespace RestAdventure.Core.Entities.StaticObjects;

public interface IStaticObjectInstance: IGameEntity
{
    StaticObject Object { get; }
}
