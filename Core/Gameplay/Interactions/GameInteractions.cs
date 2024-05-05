using System.Diagnostics.CodeAnalysis;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Interactions.Notifications;

namespace RestAdventure.Core.Gameplay.Interactions;

public class GameInteractions
{
    readonly List<InteractionInstance> _newInteractions = new();
    readonly Dictionary<CharacterId, InteractionInstance> _interactions = new();

    public GameInteractions(GameState gameState)
    {
        GameState = gameState;
    }

    internal GameState GameState { get; }

    public bool TryStartInteraction(Character character, Interaction interaction, IGameEntityWithInteractions entity, [NotNullWhen(false)] out string? whyNot)
    {
        if (entity.Interactions.Get(interaction.Id) == null)
        {
            whyNot = $"Interaction {interaction} cannot be performed on entity {entity}";
            return false;
        }

        InteractionInstance? currentInteraction = GetCharacterInteraction(character);
        if (currentInteraction != null)
        {
            whyNot = $"Character {character} is already performing an interaction";
            return false;
        }

        if (!interaction.CanInteract(character, entity))
        {
            whyNot = $"Character {character} cannot perform interaction {interaction} on entity {entity}";
            return false;
        }

        InteractionInstance instance = interaction.Instantiate(character, entity);
        _newInteractions.Add(instance);

        whyNot = null;
        return true;
    }

    public async Task ResolveInteractionsAsync(GameContent content, GameState state)
    {
        foreach (InteractionInstance newInteraction in _newInteractions)
        {
            await newInteraction.OnStartAsync(content, state);
            _interactions[newInteraction.Character.Id] = newInteraction;

            await GameState.Publisher.Publish(new InteractionStarted { InteractionInstance = newInteraction });
        }
        _newInteractions.Clear();

        foreach (InteractionInstance instance in _interactions.Values)
        {
            await instance.OnTickAsync(content, state);
        }

        List<CharacterId> toRemove = [];
        foreach ((CharacterId? characterId, InteractionInstance? instance) in _interactions)
        {
            if (instance.IsOver(content, state))
            {
                await instance.OnEndAsync(content, state);
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
