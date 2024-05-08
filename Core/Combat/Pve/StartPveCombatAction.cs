using Microsoft.Extensions.Logging;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Monsters;
using RestAdventure.Kernel.Errors;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class StartPveCombatAction : Action
{
    readonly ILogger<StartPveCombatAction> _logger;

    public StartPveCombatAction(IReadOnlyList<Character> attackers, IReadOnlyList<MonsterInstance> defenders, ILogger<StartPveCombatAction> logger) : base("start-combat")
    {
        _logger = logger;
        Attackers = attackers;
        Defenders = defenders;
    }

    public IReadOnlyList<Character> Attackers { get; }
    public IReadOnlyList<MonsterInstance> Defenders { get; }

    public override bool IsOver(GameState state, Character character) => true;

    protected override async Task OnStartAsync(GameState state, Character _)
    {
        Maybe<CombatInPreparation> combat = await state.Combats.StartCombatAsync(Attackers, Defenders);
        if (!combat.Success)
        {
            _logger.LogError("Cannot create combat: {reason}", combat.WhyNot);
            return;
        }

        foreach (Character attacker in Attackers)
        {
            state.Actions.QueueAction(attacker, new PveCombatAction(combat.Value));
        }
    }

    public override string ToString() => $"Start {string.Join(", ", Attackers)} v. {string.Join(", ", Defenders)}";
}
