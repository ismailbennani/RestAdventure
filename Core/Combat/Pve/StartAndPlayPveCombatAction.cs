using Microsoft.Extensions.Logging;
using RestAdventure.Core.Combat.Options;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Kernel.Errors;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class StartAndPlayPveCombatAction : Action
{
    public StartAndPlayPveCombatAction(MonsterGroup monsterGroup) : base("combat-started")
    {
        MonsterGroup = monsterGroup;
    }

    public MonsterGroup MonsterGroup { get; }
    public CombatInstance? CombatInstance { get; private set; }

    public override bool IsOver(GameState state, Character character) => state.Combats.GetCombatInvolving(character) == null;

    protected override async Task OnStartAsync(GameState state, Character character)
    {
        ILogger<StartAndPlayPveCombatAction> logger = state.LoggerFactory.CreateLogger<StartAndPlayPveCombatAction>();

        if (CombatInstance != null)
        {
            logger.LogError("Combat started already");
            return;
        }

        Maybe<CombatInstance> combat = await state.Combats.StartCombatAsync(
            [character],
            CombatFormationOptions.DefaultCharacterTeamOptions(state),
            [MonsterGroup],
            CombatFormationOptions.DefaultMonsterTeamOptions(state)
        );
        if (!combat.Success)
        {
            logger.LogError("Could not start combat: {reason}", combat.WhyNot);
            return;
        }

        CombatInstance = combat.Value;
        MonsterGroup.JoinCombatAction = new JoinAndPlayPveCombatAction(MonsterGroup, CombatInstance);
    }

    protected override Task OnEndAsync(GameState state, Character character)
    {
        CombatInstance = null;
        MonsterGroup.JoinCombatAction = null;

        return Task.CompletedTask;
    }
}
