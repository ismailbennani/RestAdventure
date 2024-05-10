using Microsoft.Extensions.Logging;
using RestAdventure.Core.Combat.Options;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Items;
using RestAdventure.Kernel.Errors;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatAction : Action
{
    CombatFormationOptions? _attackersOptions;
    IReadOnlyList<MonsterCombatInstance>? _monsters;
    readonly CombatFormationOptions? _monstersOptions;
    bool _monsterGroupKilled;

    public PveCombatAction(MonsterGroup monsterGroup) : base("pve-combat")
    {
        MonsterGroup = monsterGroup;
        _monstersOptions = new CombatFormationOptions { Accessibility = CombatFormationAccessibility.TeamOnly };

        ExperienceReward = MonsterGroup.Monsters.Sum(m => m.Species.Experience);
        ItemsReward = MonsterGroup.Monsters.Aggregate(Enumerable.Empty<ItemStack>(), (items, m) => items.Concat(m.Species.Items.Concat(m.Species.Family.Items))).ToArray();
    }

    public IReadOnlyList<Character> Attackers =>
        CombatInPreparation?.Attackers.Entities.OfType<Character>().ToArray() ?? Combat?.Attackers.Entities.OfType<Character>().ToArray() ?? Array.Empty<Character>();

    public CombatFormationOptions AttackersOptions => CombatInPreparation?.Attackers.Options ?? _attackersOptions ?? CombatFormationOptions.Default;

    public MonsterGroup MonsterGroup { get; }
    public int ExperienceReward { get; }
    public ItemStack[] ItemsReward { get; }

    public CombatInPreparation? CombatInPreparation { get; private set; }
    public CombatInstance? Combat { get; private set; }

    public bool Interrupt { get; private set; }

    public Maybe SetOptions(CombatFormationOptions options)
    {
        //FIXME: hard ot use

        _attackersOptions = options;
        return true;
    }

    protected override Maybe CanPerformInternal(GameState state, Character character) =>
        CombatInPreparation == null ? state.Combats.CanStartCombat([character], MonsterGroup) : state.Combats.CanJoinCombat(character, CombatInPreparation, CombatSide.Attackers);

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
            Team team = new();
            _monsters = MonsterGroup.Monsters.Select(m => new MonsterCombatInstance(team, m.Species, m.Level, character.Location)).ToArray();

            foreach (MonsterCombatInstance monster in _monsters)
            {
                await state.Entities.AddAsync(monster);
            }

            Maybe<CombatInPreparation> combat = await state.Combats.StartCombatPreparationAsync(
                [character],
                _attackersOptions ?? CombatFormationOptions.Default,
                _monsters,
                _monstersOptions ?? CombatFormationOptions.Default
            );
            if (!combat.Success)
            {
                logger.LogError("Combat creation failed: {reason}", combat.WhyNot);
                Interrupt = true;
                return;
            }

            foreach (MonsterCombatInstance monster in _monsters)
            {
                monster.Busy = true;
            }

            CombatInPreparation = combat.Value;
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

    protected override async Task OnEndAsync(GameState state, Character character)
    {
        if (Combat?.Winner == null)
        {
            return;
        }

        switch (Combat.Winner)
        {
            case CombatSide.Attackers:
                if (ExperienceReward > 0)
                {
                    character.Progression.Progress(ExperienceReward);
                }

                character.Inventory.Add(ItemsReward);

                if (!_monsterGroupKilled)
                {
                    await MonsterGroup.KillAsync(state);
                    _monsterGroupKilled = true;
                }

                break;
            case CombatSide.Defenders:
                // kill spawned monsters in all cases, even if they win. The monster group remains in this case
                if (_monsters != null)
                {
                    foreach (MonsterCombatInstance monster in _monsters)
                    {
                        await monster.KillAsync(state);
                    }

                    _monsters = null;
                }
                break;
        }
    }

    public override string ToString() => $"{string.Join(", ", Attackers)} v. {string.Join(", ", _monsters ?? [])}";
}
