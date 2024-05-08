using Microsoft.Extensions.Logging;
using RestAdventure.Core.Characters;
using RestAdventure.Kernel.Errors;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatAction : Action
{
    IReadOnlyList<IGameEntityWithCombatStatistics>? _attackers;
    IReadOnlyList<IGameEntityWithCombatStatistics>? _defenders;
    readonly ILogger<PveCombatAction> _logger;

    public PveCombatAction(
        IReadOnlyList<IGameEntityWithCombatStatistics> attackers,
        IReadOnlyList<IGameEntityWithCombatStatistics> defenders,
        ILogger<PveCombatAction> logger
    ) : base("combat")
    {
        _attackers = attackers;
        _defenders = defenders;
        _logger = logger;

    }

    public PveCombatAction(CombatInPreparation combatInPreparation, ILogger<PveCombatAction> logger) : base("combat")
    {
        CombatInPreparation = combatInPreparation;
        _logger = logger;
    }

    public IReadOnlyList<IGameEntityWithCombatStatistics> Attackers =>
        _attackers ?? CombatInPreparation?.Attackers.Entities ?? Combat?.Attackers.Entities ?? Array.Empty<IGameEntityWithCombatStatistics>();

    public IReadOnlyList<IGameEntityWithCombatStatistics> Defenders =>
        _defenders ?? CombatInPreparation?.Defenders.Entities ?? Combat?.Defenders.Entities ?? Array.Empty<IGameEntityWithCombatStatistics>();

    public CombatInPreparation? CombatInPreparation { get; private set; }
    public CombatInstance? Combat { get; private set; }
    public bool Failed { get; private set; }

    public override bool IsOver(GameState state, Character character) => Failed || CombatInPreparation is { Canceled: true } || Combat is { IsOver: true };

    protected override async Task OnStartAsync(GameState state, Character character)
    {
        if (CombatInPreparation == null)
        {
            if (_attackers == null || _defenders == null)
            {
                _logger.LogError("Combat creation failed: no attackers or no defenders");
                Failed = true;
                return;
            }

            Maybe<CombatInPreparation> combat = await state.Combats.StartCombatAsync(_attackers, _defenders);
            if (!combat.Success)
            {
                _logger.LogError("Combat creation failed: {reason}", combat.WhyNot);
                Failed = true;
                return;
            }

            CombatInPreparation = combat.Value;
            _attackers = null;
            _defenders = null;
        }
        else
        {
            Maybe added = CombatInPreparation.Attackers.Add(character);
            if (!added.Success)
            {
                Failed = true;
                _logger.LogError("Join combat failed: {reason}", added.WhyNot);
            }
        }
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

    public override string ToString() => $"{string.Join(", ", Attackers)} v. {string.Join(", ", Defenders)}";
}
