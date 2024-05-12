using Microsoft.Extensions.Logging;
using RestAdventure.Core.Combat.Options;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Kernel.Errors;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class StartAndPlayPveCombatAction : Action
{
    public StartAndPlayPveCombatAction(MonsterGroupId monsterGroupId) : base("combat-started")
    {
        MonsterGroupId = monsterGroupId;
    }

    public MonsterGroupId MonsterGroupId { get; }
    public CombatInstance? CombatInstance { get; private set; }

    protected override Maybe CanPerformInternal(Game state, Character character)
    {
        MonsterGroup? monsterGroup = state.Entities.Get<MonsterGroup>(MonsterGroupId);
        if (monsterGroup == null)
        {
            return "Could not find monster group";
        }

        return state.Combats.CanStartCombat([character], [monsterGroup]);
    }

    public override bool IsOver(Game state, Character character) => state.Combats.GetCombatInvolving(character) == null;

    protected override async Task OnStartAsync(Game state, Character character)
    {
        ILogger<StartAndPlayPveCombatAction> logger = state.LoggerFactory.CreateLogger<StartAndPlayPveCombatAction>();

        MonsterGroup? monsterGroup = state.Entities.Get<MonsterGroup>(MonsterGroupId);
        if (monsterGroup == null)
        {
            logger.LogError("Could not find monster group");
            return;
        }

        Maybe<CombatInstance> combat = await state.Combats.StartCombatAsync(
            [character],
            CombatFormationOptions.DefaultCharacterTeamOptions(state),
            [monsterGroup],
            CombatFormationOptions.DefaultMonsterTeamOptions(state)
        );
        if (!combat.Success)
        {
            logger.LogError("Could not start combat: {reason}", combat.WhyNot);
            return;
        }

        CombatInstance = combat.Value;
        monsterGroup.OngoingCombat = CombatInstance;
    }

    protected override Task OnEndAsync(Game state, Character character)
    {
        CombatInstance = null;

        MonsterGroup? monsterGroup = state.Entities.Get<MonsterGroup>(MonsterGroupId);
        if (monsterGroup != null)
        {
            monsterGroup.OngoingCombat = null;
        }

        return Task.CompletedTask;
    }
}
