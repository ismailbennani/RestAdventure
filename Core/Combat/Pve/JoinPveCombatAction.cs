using RestAdventure.Core.Characters;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class JoinPveCombatAction : Action
{
    public JoinPveCombatAction(CombatInPreparation combat) : base("join-combat")
    {
        Combat = combat;
    }

    public CombatInPreparation Combat { get; }

    public override bool IsOver(GameState state, Character character) => true;

    protected override Task OnStartAsync(GameState state, Character character)
    {
        CombatFormationInPreparation team = Combat.GetTeam(CombatSide.Attackers);
        team.Add(character);

        state.Actions.QueueAction(character, new PveCombatAction(Combat));
        return Task.CompletedTask;
    }

    public override string ToString() => $"Join combat {Combat}";
}
