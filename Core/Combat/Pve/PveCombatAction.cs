﻿using Microsoft.Extensions.Logging;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Kernel.Errors;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatAction : Action
{
    IReadOnlyList<MonsterInstance>? _defenders;
    readonly ILogger<PveCombatAction> _logger;

    public PveCombatAction(IReadOnlyList<MonsterInstance> defenders, ILogger<PveCombatAction> logger) : base("combat")
    {
        _defenders = defenders;
        _logger = logger;

    }

    public PveCombatAction(CombatInPreparation combatInPreparation, ILogger<PveCombatAction> logger) : base("combat")
    {
        CombatInPreparation = combatInPreparation;
        _logger = logger;
    }

    public IReadOnlyList<Character> Attackers =>
        CombatInPreparation?.Attackers.Entities.OfType<Character>().ToArray() ?? Combat?.Attackers.Entities.OfType<Character>().ToArray() ?? Array.Empty<Character>();

    public IReadOnlyList<MonsterInstance> Defenders =>
        _defenders
        ?? CombatInPreparation?.Defenders.Entities.OfType<MonsterInstance>().ToArray()
        ?? Combat?.Defenders.Entities.OfType<MonsterInstance>().ToArray() ?? Array.Empty<MonsterInstance>();

    public CombatInPreparation? CombatInPreparation { get; private set; }
    public CombatInstance? Combat { get; private set; }
    public bool Interrupt { get; private set; }

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

    public override bool IsOver(GameState state, Character character) => Interrupt || Combat is { Over: true };

    protected override async Task OnStartAsync(GameState state, Character character)
    {
        if (CombatInPreparation == null)
        {
            if (_defenders == null)
            {
                _logger.LogError("Combat creation failed: no defenders");
                Interrupt = true;
                return;
            }

            Maybe<CombatInPreparation> combat = await state.Combats.StartCombatPreparationAsync([character], _defenders);
            if (!combat.Success)
            {
                _logger.LogError("Combat creation failed: {reason}", combat.WhyNot);
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
                _logger.LogError("Join combat failed: {reason}", added.WhyNot);
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
