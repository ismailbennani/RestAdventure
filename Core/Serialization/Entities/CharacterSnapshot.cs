using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Serialization.Jobs;
using RestAdventure.Core.Utils;
using RestAdventure.Kernel.Security;

namespace RestAdventure.Core.Serialization.Entities;

public class CharacterSnapshot : GameEntitySnapshot<CharacterId>, ICharacter
{
    CharacterSnapshot(CharacterId id) : base(id)
    {
    }

    public required UserId PlayerId { get; init; }
    public required CharacterClass Class { get; init; }
    public required int Health { get; init; }
    public required ProgressionBarSnapshot Progression { get; init; }
    IProgressionBar ICharacter.Progression => Progression;
    public required InventorySnapshot Inventory { get; init; }
    public required IReadOnlyCollection<JobInstanceSnapshot> Jobs { get; init; }
    IReadOnlyCollection<IJobInstance> ICharacter.Jobs => Jobs;

    public static CharacterSnapshot Take(Character character) =>
        new(character.Id)
        {
            Team = character.Team == null ? null : TeamSnapshot.Take(character.Team),
            Name = character.Name,
            Location = character.Location,
            Busy = character.Busy,
            PlayerId = character.Player.User.Id,
            Class = character.Class,
            Health = character.Health,
            Progression = ProgressionBarSnapshot.Take(character.Progression),
            Inventory = InventorySnapshot.Take(character.Inventory),
            Jobs = character.Jobs.Select(JobInstanceSnapshot.Take).ToArray()
        };
}
