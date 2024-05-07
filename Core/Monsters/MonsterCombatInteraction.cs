using RestAdventure.Core.Characters;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Interactions;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Monsters;

public class MonsterCombatInteraction : CombatInteraction
{
    protected override Task<Maybe> CanInteractInternalAsync(GameState state, Character character, IInteractibleEntity target)
    {
        if (target is not MonsterInstance monster)
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
