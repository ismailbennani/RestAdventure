using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Entities.Monsters;

public class GameMonsters
{
    public GameResourcesStore<MonsterFamilyId, MonsterFamily> Families { get; } = new();
    public GameResourcesStore<MonsterSpeciesId, MonsterSpecies> Species { get; } = new();
}
