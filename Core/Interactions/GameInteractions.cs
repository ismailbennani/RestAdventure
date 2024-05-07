using RestAdventure.Core.Characters;
using RestAdventure.Core.Interactions.Notifications;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Interactions;

public class GameInteractions
{
    readonly List<InteractionInstance> _newInteractions = new();
    readonly Dictionary<CharacterId, InteractionInstance> _interactions = new();

    public GameInteractions(GameState gameState)
    {
        GameState = gameState;
    }

    internal GameState GameState { get; }

    public async Task<Maybe<InteractionInstance>> StartInteractionAsync(Character character, Interaction interaction, IInteractibleEntity entity)
    {
        Maybe canInteract = await interaction.CanInteractAsync(GameState, character, entity);
        if (!canInteract.Success)
        {
            return $"Character {character} cannot perform interaction {interaction} on entity {entity}: {canInteract.WhyNot}";
        }

        Maybe<InteractionInstance> instance = await interaction.InstantiateInteractionAsync(GameState, character, entity);
        if (instance.Success)
        {
            _newInteractions.Add(instance.Value);
        }

        return instance;
    }

    public async Task ResolveInteractionsAsync(GameState state)
    {
        foreach (InteractionInstance newInteraction in _newInteractions)
        {
            await newInteraction.OnStartAsync(state);
            _interactions[newInteraction.Character.Id] = newInteraction;

            await GameState.Publisher.Publish(new InteractionStarted { InteractionInstance = newInteraction });
        }
        _newInteractions.Clear();

        foreach (InteractionInstance instance in _interactions.Values)
        {
            await instance.OnTickAsync(state);
        }

        List<CharacterId> toRemove = [];
        foreach ((CharacterId? characterId, InteractionInstance? instance) in _interactions)
        {
            if (instance.IsOver(state))
            {
                await instance.OnEndAsync(state);
                toRemove.Add(characterId);
            }
        }

        foreach (CharacterId characterId in toRemove)
        {
            if (!_interactions.Remove(characterId, out InteractionInstance? instance))
            {
                continue;
            }

            await GameState.Publisher.Publish(new InteractionEnded { InteractionInstance = instance });
        }
    }

    public InteractionInstance? GetCharacterInteraction(Character character) => _interactions.GetValueOrDefault(character.Id);
}
