using Microsoft.Extensions.Logging;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Kernel.Errors;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class JoinAndPlayPveCombatAction : Action
{
    public JoinAndPlayPveCombatAction(MonsterGroupId monsterGroupId, CombatInstanceId combatId) : base("combat-started")
    {
        MonsterGroupId = monsterGroupId;
        CombatId = combatId;
    }

    public MonsterGroupId MonsterGroupId { get; }
    public CombatInstanceId CombatId { get; set; }

    protected override Maybe CanPerformInternal(Game state, Character character)
    {
        MonsterGroup? monsterGroup = state.Entities.Get<MonsterGroup>(MonsterGroupId);
        if (monsterGroup == null)
        {
            return "Could not find monster group";
        }

        CombatInstance? combat = state.Combats.GetCombatInvolving(monsterGroup);
        if (combat == null)
        {
            return "Combat not started";
        }

        return combat.Attackers.CanAdd(character);
    }

    public override bool IsOver(Game state, Character character) => state.Combats.GetCombatInvolving(character) == null;

    protected override Task OnStartAsync(Game state, Character character)
    {
        ILogger<StartAndPlayPveCombatAction> logger = state.LoggerFactory.CreateLogger<StartAndPlayPveCombatAction>();

        MonsterGroup? monsterGroup = state.Entities.Get<MonsterGroup>(MonsterGroupId);
        if (monsterGroup == null)
        {
            logger.LogError("Could not find monster group");
            return Task.CompletedTask;
        }

        CombatInstance? combat = state.Combats.GetCombatInvolving(monsterGroup);
        if (combat == null)
        {
            logger.LogError("Could not find combat involving monsters {monsters}", monsterGroup);
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
