using RestAdventure.Core.Characters;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Monsters;

public class MonsterCombatInteraction : CombatInteraction
{
    public override Task<Maybe> CanInteractAsync(GameState state, Character character, IGameEntity entity)
    {
        if (entity is not MonsterInstance monster)
        {
            return Task.FromResult<Maybe>("Target is not a monster");
        }

        if (character.Location != monster.Location)
        {
            return Task.FromResult<Maybe>("Monster is inaccessible");
        }

        return Task.FromResult<Maybe>(true);
    }
}
