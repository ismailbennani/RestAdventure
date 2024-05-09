using Microsoft.Extensions.Logging;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Combat.Options;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Items;
using RestAdventure.Core.Simulation;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Combat;

public class CombatSimulationWorkProvider : SimulationWorkProvider
{
    readonly GameState _state;
    readonly ILogger<CombatSimulationWorkProvider> _logger;

    public CombatSimulationWorkProvider(GameState state)
    {
        _state = state;
        _logger = state.LoggerFactory.CreateLogger<CombatSimulationWorkProvider>();
    }

    public override IEnumerable<GameSimulation.Work> PreTick()
    {
        foreach (CombatInPreparation combat in _state.Combats.InPreparation)
        {
            yield return new GameSimulation.Work(combat.Id.Guid, () => EnforceCombatOptionsAsync(combat));
        }
    }

    public override IEnumerable<GameSimulation.Work> Early()
    {
        foreach (CombatInPreparation combat in _state.Combats.InPreparation)
        {
            yield return new GameSimulation.Work(combat.Id.Guid, () => StartCombatIfReadyAsync(combat));
        }
    }

    public override IEnumerable<GameSimulation.Work> Tick()
    {
        foreach (CombatInPreparation combat in _state.Combats.InPreparation)
        {
            yield return new GameSimulation.Work(combat.Id.Guid, () => TickCombatsInPreparationAsync(combat));
        }

        foreach (CombatInstance combat in _state.Combats)
        {
            yield return new GameSimulation.Work(combat.Id.Guid, () => combat.PlayTurnAsync());
        }
    }

    public override IEnumerable<GameSimulation.Work> Late()
    {
        foreach (CombatInstance combat in _state.Combats.Where(c => c.Over))
        {
            yield return new GameSimulation.Work(combat.Id.Guid, () => ResolveCombatIfOverAsync(combat));
        }
    }

    Task TickCombatsInPreparationAsync(CombatInPreparation combatInPreparation)
    {
        combatInPreparation.Turn++;
        return Task.CompletedTask;
    }

    Task EnforceCombatOptionsAsync(CombatInPreparation combatInPreparation)
    {
        KickPlayersIfNecessary(combatInPreparation.Attackers);
        KickPlayersIfNecessary(combatInPreparation.Defenders);
        return Task.CompletedTask;
    }

    async Task StartCombatIfReadyAsync(CombatInPreparation combatInPreparation)
    {
        if (combatInPreparation.Turn < _state.Settings.Combat.PreparationPhaseDuration)
        {
            return;
        }

        Maybe<CombatInstance> startCombat = await _state.Combats.StartCombatAsync(combatInPreparation);
        if (!startCombat.Success)
        {
            _logger.LogError("Could not start combat: {reason}", startCombat.WhyNot);
        }
    }

    async Task ResolveCombatIfOverAsync(CombatInstance combat)
    {
        if (!combat.Over)
        {
            return;
        }

        if (combat.Winner.HasValue)
        {
            CombatFormation winnerTeam = combat.GetTeam(combat.Winner.Value);
            CombatFormation loserTeam = combat.GetTeam(combat.Winner.Value.OtherSide());

            int baseExperience = loserTeam.Entities.OfType<MonsterInstance>().Sum(m => m.Species.Experience);
            ItemStack[] loot = loserTeam.Entities.OfType<MonsterInstance>()
                .Aggregate(Enumerable.Empty<ItemStack>(), (items, m) => items.Concat(m.Species.Items.Concat(m.Species.Family.Items)))
                .ToArray();

            IEnumerable<Character> characters = winnerTeam.Entities.OfType<Character>();
            foreach (Character character in characters)
            {
                if (baseExperience > 0)
                {
                    character.Progression.Progress(baseExperience);
                }

                character.Inventory.Add(loot);
            }

            foreach (IGameEntityWithCombatStatistics entity in loserTeam.Entities)
            {
                await entity.KillAsync(_state);
            }
        }

        await _state.Publisher.Publish(new CombatEnded { Combat = combat });

        _state.Combats.RemoveCombat(combat);
    }

    void KickPlayersIfNecessary(CombatFormationInPreparation formations)
    {
        switch (formations.Options.Accessibility)
        {
            case CombatFormationAccessibility.TeamOnly:
                foreach (IGameEntityWithCombatStatistics entityInCombat in formations.Entities.ToArray())
                {
                    if (entityInCombat.Team != formations.Owner.Team)
                    {
                        formations.Remove(entityInCombat);
                    }
                }
                break;
        }
    }
}
