using RestAdventure.Core.Interactions;

namespace RestAdventure.Core.Combat;

public class CharacterCombatInteractionInstance : InteractionInstance
{
    public CombatInPreparation CombatInPreparation { get; }
    public CombatInstance? Combat { get; private set; }

    public CharacterCombatInteractionInstance(CombatInPreparation combatInPreparation, IInteractingEntity source, Interaction interaction, IInteractibleEntity target) : base(
        source,
        interaction,
        target
    )
    {
        CombatInPreparation = combatInPreparation;
    }

    public override bool IsOver(GameState state) => CombatInPreparation is { Canceled: true } || Combat is { IsOver: true };

    public override Task OnTickAsync(GameState state)
    {
        Combat = state.Combats.Get(CombatInPreparation.Id);
        return Task.CompletedTask;
    }

    public override async Task OnEndAsync(GameState state)
    {
        if (Combat is not { Winner: not null })
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
