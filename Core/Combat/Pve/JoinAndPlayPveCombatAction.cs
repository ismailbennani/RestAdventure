using Microsoft.Extensions.Logging;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Kernel.Errors;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class JoinAndPlayPveCombatAction : Action
{
    public JoinAndPlayPveCombatAction(MonsterGroup monsterGroup, CombatInstance combat) : base("combat-started")
    {
        MonsterGroup = monsterGroup;
        Combat = combat;
    }

    public MonsterGroup MonsterGroup { get; }
    public CombatInstance Combat { get; }

    protected override Maybe CanPerformInternal(GameState state, Character character)
    {
        CombatInstance? combat = state.Combats.GetCombatInvolving(MonsterGroup);
        if (combat == null)
        {
            return "Combat not started";
        }

        return combat.Attackers.CanAdd(character);
    }

    public override bool IsOver(GameState state, Character character) => state.Combats.GetCombatInvolving(character) == null;

    protected override Task OnStartAsync(GameState state, Character character)
    {
        ILogger<StartAndPlayPveCombatAction> logger = state.LoggerFactory.CreateLogger<StartAndPlayPveCombatAction>();

        CombatInstance? combat = state.Combats.GetCombatInvolving(MonsterGroup);
        if (combat == null)
        {
            logger.LogError("Could not find combat involving monsters {monsters}", MonsterGroup);
            return Task.CompletedTask;
        }

        Maybe joined = combat.Attackers.Add(character);
        if (!joined.Success)
        {
            logger.LogError("Could not join combat: {reason}", joined.WhyNot);
        }
        return Task.CompletedTask;
    }
}
