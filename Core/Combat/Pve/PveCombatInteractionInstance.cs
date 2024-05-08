using RestAdventure.Core.Interactions;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatInteractionInstance : InteractionInstance
{
    public CombatInPreparation CombatInPreparation { get; }
    public CombatInstance? Combat { get; private set; }

    public PveCombatInteractionInstance(CombatInPreparation combatInPreparation, IInteractingEntity source, Interaction interaction, IInteractibleEntity target) : base(
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
        Combat ??= state.Combats.Get(CombatInPreparation.Id);
        return Task.CompletedTask;
    }
}
