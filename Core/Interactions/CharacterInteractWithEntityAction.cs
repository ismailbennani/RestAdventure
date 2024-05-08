using RestAdventure.Core.Actions;
using RestAdventure.Core.Characters;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Interactions;

public class CharacterInteractWithEntityAction : CharacterAction
{
    public CharacterInteractWithEntityAction(Interaction interaction, IInteractibleEntity target)
    {
        Interaction = interaction;
        Target = target;
    }

    public Interaction Interaction { get; }
    public IInteractibleEntity Target { get; }

    protected override Maybe CanPerformInternal(GameState state, Character character) => Interaction.CanInteract(character, Target);

    public override async Task<Maybe> PerformAsync(GameState state, Character character) => await state.Interactions.StartInteractionAsync(character, Interaction, Target);

    public override string ToString() => Interaction.ToString();
}
