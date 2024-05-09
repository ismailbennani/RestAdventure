using Microsoft.Extensions.Logging;
using RestAdventure.Core.Combat.Options;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Kernel.Errors;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatAction : Action
{
    CombatFormationOptions? _attackersOptions;
    CombatFormationOptions? _defendersOptions;
    IReadOnlyList<MonsterInstance>? _defenders;

    public PveCombatAction(IReadOnlyList<MonsterInstance> defenders) : base("combat")
    {
        _defenders = defenders;

    }

    public PveCombatAction(CombatInPreparation combatInPreparation) : base("combat")
    {
        CombatInPreparation = combatInPreparation;
    }

    public IReadOnlyList<Character> Attackers =>
        CombatInPreparation?.Attackers.Entities.OfType<Character>().ToArray() ?? Combat?.Attackers.Entities.OfType<Character>().ToArray() ?? Array.Empty<Character>();

    public CombatFormationOptions AttackersOptions => CombatInPreparation?.Attackers.Options ?? _attackersOptions ?? CombatFormationOptions.Default;

    public IReadOnlyList<MonsterInstance> Defenders =>
        _defenders
        ?? CombatInPreparation?.Defenders.Entities.OfType<MonsterInstance>().ToArray()
        ?? Combat?.Defenders.Entities.OfType<MonsterInstance>().ToArray() ?? Array.Empty<MonsterInstance>();

    public CombatFormationOptions DefendersOptions => CombatInPreparation?.Defenders.Options ?? _attackersOptions ?? CombatFormationOptions.Default;

    public CombatInPreparation? CombatInPreparation { get; private set; }
    public CombatInstance? Combat { get; private set; }

    public bool Interrupt { get; private set; }

    public Maybe SetOptions(CombatSide side, CombatFormationOptions options)
    {
        //FIXME: hard ot use

        if (CombatInPreparation == null)
        {
            switch (side)
            {
                case CombatSide.Attackers:
                    _attackersOptions = options;
                    break;
                case CombatSide.Defenders:
                    _defendersOptions = options;
                    break;
            }
            return true;
        }

        return "Combat is in preparation";
    }

    protected override Maybe CanPerformInternal(GameState state, Character character)
    {
        if (CombatInPreparation != null)
        {
            return state.Combats.CanJoinCombat(character, CombatInPreparation, CombatSide.Attackers);
        }

        if (_defenders != null)
        {
            return state.Combats.CanStartCombat([character], _defenders);
        }

        return "Internal error";
    }

    public override bool IsOver(GameState state, Character character) =>
        // interrupted
        Interrupt
        // character kicked from combat in preparation
        || CombatInPreparation != null && !CombatInPreparation.Attackers.Entities.Contains(character) && !CombatInPreparation.Defenders.Entities.Contains(character)
        // combat over
        || Combat is { Over: true };

    protected override async Task OnStartAsync(GameState state, Character character)
    {
        ILogger<PveCombatAction> logger = state.LoggerFactory.CreateLogger<PveCombatAction>();

        if (CombatInPreparation == null)
        {
            if (_defenders == null)
            {
                logger.LogError("Combat creation failed: no defenders");
                Interrupt = true;
                return;
            }

            Maybe<CombatInPreparation> combat = await state.Combats.StartCombatPreparationAsync(
                [character],
                _attackersOptions ?? CombatFormationOptions.Default,
                _defenders,
                _defendersOptions ?? CombatFormationOptions.Default
            );
            if (!combat.Success)
            {
                logger.LogError("Combat creation failed: {reason}", combat.WhyNot);
                Interrupt = true;
                return;
            }

            CombatInPreparation = combat.Value;

            foreach (MonsterInstance defender in _defenders)
            {
                defender.Busy = true;
            }
            _defenders = null;
        }
        else
        {
            Maybe added = CombatInPreparation.Attackers.Add(character);
            if (!added.Success)
            {
                Interrupt = true;
                logger.LogError("Join combat failed: {reason}", added.WhyNot);
            }
        }
    }

    protected override Task OnTickAsync(GameState state, Character character)
    {
        if (CombatInPreparation is { Started: true })
        {
            CombatInstance? combat = state.Combats.GetCombat(CombatInPreparation.Id);
            if (combat != null)
            {
                Combat = combat;
            }
            else
            {
                Interrupt = true;
            }
        }

        return Task.CompletedTask;
    }

    public override string ToString() => $"{string.Join(", ", Attackers)} v. {string.Join(", ", Defenders)}";
}
