using RestAdventure.Core.Characters;
using RestAdventure.Core.Interactions;

namespace RestAdventure.Core.Combat;

public class CharacterCombatInteractionInstance : InteractionInstance
{
    public CombatInstance Combat { get; }

    public CharacterCombatInteractionInstance(CombatInstance combat, Character character, Interaction interaction, IInteractibleEntity target) : base(
        character,
        interaction,
        target
    )
    {
        Combat = combat;
    }

    public override bool IsOver(GameState state) => Combat.IsOver;

    public override async Task OnTickAsync(GameState state) => await Combat.PlayTurnAsync();

    public override async Task OnEndAsync(GameState state)
    {
        if (!Combat.Winner.HasValue)
        {
            return;
        }

        CombatFormation loserTeam = Combat.GetTeam(Combat.Winner.Value.OtherSide());
        foreach (IGameEntityWithCombatStatistics entity in loserTeam.Entities)
        {
            await entity.KillAsync(state);
        }
    }
}
