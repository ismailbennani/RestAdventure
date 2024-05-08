using RestAdventure.Core.Characters;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.Items;
using RestAdventure.Core.Monsters;

namespace RestAdventure.Core.Combat.Pve;

public class PveCombatInteractionInstance : InteractionInstance
{
    public CombatInPreparation CombatInPreparation { get; }
    public CombatInstance? Combat { get; private set; }

    public PveCombatInteractionInstance(CombatInPreparation combatInPreparation, IInteractingEntity source, Interaction interaction, IInteractibleEntity target) : base(
        source,
        interaction,
        target
    )
    {
        CombatInPreparation = combatInPreparation;
    }

    public override bool IsOver(GameState state) => CombatInPreparation is { Canceled: true } || Combat is { IsOver: true };

    public override Task OnTickAsync(GameState state)
    {
        Combat ??= state.Combats.Get(CombatInPreparation.Id);
        return Task.CompletedTask;
    }

    public override async Task OnEndAsync(GameState state)
    {
        if (Combat is not { Winner: not null })
        {
            return;
        }

        switch (Combat.Winner)
        {
            case CombatSide.Attackers:
                MonsterInstance[] monsters = Combat.Defenders.Entities.OfType<MonsterInstance>().ToArray();
                int baseExperience = monsters.Sum(m => m.Species.Experience);
                ItemStack[] loot = monsters.Aggregate(Enumerable.Empty<ItemStack>(), (items, m) => items.Concat(m.Species.Items.Concat(m.Species.Family.Items))).ToArray();

                IEnumerable<Character> characters = Combat.Attackers.Entities.OfType<Character>();
                foreach (Character character in characters)
                {
                    if (baseExperience > 0)
                    {
                        character.Progression.Progress(baseExperience);
                    }

                    character.Inventory.Add(loot);
                }

                foreach (MonsterInstance monster in monsters)
                {
                    await monster.KillAsync(state);
                }
                break;
            case CombatSide.Defenders:
                foreach (IGameEntityWithCombatStatistics entity in Combat.Attackers.Entities)
                {
                    await entity.KillAsync(state);
                }
                break;
        }
    }
}
