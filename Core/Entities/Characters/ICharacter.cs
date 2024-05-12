using RestAdventure.Core.Jobs;
using RestAdventure.Core.Utils;

namespace RestAdventure.Core.Entities.Characters;

public interface ICharacter : IGameEntity
{
    /// <summary>
    ///     The class of the character
    /// </summary>
    CharacterClass Class { get; }

    /// <summary>
    ///     The health of the character
    /// </summary>
    int Health { get; }

    /// <summary>
    ///     The progression of the character
    /// </summary>
    IProgressionBar Progression { get; }

    /// <summary>
    ///     The jobs of the character
    /// </summary>
    IReadOnlyCollection<IJobInstance> Jobs { get; }
}
