namespace RestAdventure.Core.Entities.Monsters;

public class MonsterInGroup
{
    public required MonsterSpecies Species { get; init; }
    public required int Level { get; init; }
}
