using RestAdventure.Core.Characters;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatAction : Action
{
    public PveCombatAction(CombatInPreparation combatInPreparation) : base("combat")
    {
        CombatInPreparation = combatInPreparation;
    }

    public IReadOnlyList<IGameEntityWithCombatStatistics> Attackers =>
        CombatInPreparation?.Attackers.Entities ?? Combat?.Attackers.Entities ?? Array.Empty<IGameEntityWithCombatStatistics>();

    public IReadOnlyList<IGameEntityWithCombatStatistics> Defenders =>
        CombatInPreparation?.Defenders.Entities ?? Combat?.Defenders.Entities ?? Array.Empty<IGameEntityWithCombatStatistics>();

    public CombatInPreparation? CombatInPreparation { get; private set; }
    public CombatInstance? Combat { get; private set; }

    public override bool IsOver(GameState state, Character character) => CombatInPreparation is { Canceled: true } || Combat is { IsOver: true };

    protected override Task OnTickAsync(GameState state, Character character)
    {
        if (CombatInPreparation != null)
        {
            CombatInstance? combat = state.Combats.Get(CombatInPreparation.Id);
            if (combat != null)
            {
                Combat = combat;
                CombatInPreparation = null;
            }
        }

        return Task.CompletedTask;
    }

    public override string ToString() => $"{string.Join(", ", Attackers)} v. {string.Join(", ", Defenders)}";
}
