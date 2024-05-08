using RestAdventure.Core.Characters;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatAction : Action
{
    IReadOnlyList<IGameEntityWithCombatStatistics>? _attackers;
    IReadOnlyList<IGameEntityWithCombatStatistics>? _defenders;

    public PveCombatAction(IReadOnlyList<IGameEntityWithCombatStatistics> attackers, IReadOnlyList<IGameEntityWithCombatStatistics> defenders) : base("combat")
    {
        _attackers = attackers;
        _defenders = defenders;
    }

    public PveCombatAction(CombatInPreparation combatInPreparation) : base("combat")
    {
        CombatInPreparation = combatInPreparation;
    }

    public PveCombatAction(CombatInstance combat) : base("combat")
    {
        Combat = combat;
    }

    public IReadOnlyList<IGameEntityWithCombatStatistics> Attackers =>
        _attackers ?? CombatInPreparation?.Attackers.Entities ?? Combat?.Attackers.Entities ?? Array.Empty<IGameEntityWithCombatStatistics>();

    public IReadOnlyList<IGameEntityWithCombatStatistics> Defenders =>
        _defenders ?? CombatInPreparation?.Defenders.Entities ?? Combat?.Defenders.Entities ?? Array.Empty<IGameEntityWithCombatStatistics>();

    public CombatInPreparation? CombatInPreparation { get; private set; }
    public CombatInstance? Combat { get; private set; }

    public override bool IsOver(GameState state, Character character) => CombatInPreparation is { Canceled: true } || Combat is { IsOver: true };

    protected override async Task OnStartAsync(GameState state, Character character)
    {
        if (_attackers == null || _defenders == null)
        {
            throw new InvalidOperationException("INTERNAL ERROR: cannot start combat");
        }

        CombatInPreparation = await state.Combats.StartCombatAsync(_attackers, _defenders);
        _attackers = null;
        _defenders = null;
    }

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
}
