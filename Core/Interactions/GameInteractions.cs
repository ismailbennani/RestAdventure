using MediatR;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Interactions.Notifications;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Interactions;

public class GameInteractions
{
    readonly IPublisher _publisher;
    readonly ILogger<GameInteractions> _logger;
    readonly List<InteractionInstance> _newInteractions = new();
    readonly Dictionary<GameEntityId, InteractionInstance> _interactions = new();

    public GameInteractions(IPublisher publisher, ILogger<GameInteractions> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Maybe<InteractionInstance>> StartInteractionAsync(IInteractingEntity source, Interaction interaction, IInteractibleEntity entity)
    {
        Maybe canInteract = interaction.CanInteract(source, entity);
        if (!canInteract.Success)
        {
            return $"Character {source} cannot perform interaction {interaction} on entity {entity}: {canInteract.WhyNot}";
        }

        Maybe<InteractionInstance> instance = await interaction.InstantiateInteractionAsync(source, entity);
        if (instance.Success)
        {
            _newInteractions.Add(instance.Value);
            source.CurrentInteraction = instance.Value;
        }

        return instance;
    }

    public async Task ResolveInteractionsAsync(GameState state)
    {
        foreach (InteractionInstance newInteraction in _newInteractions)
        {
            await newInteraction.OnStartAsync(state);
            _interactions[newInteraction.Source.Id] = newInteraction;

            await _publisher.Publish(new InteractionStarted { InteractionInstance = newInteraction });
        }
        _newInteractions.Clear();

        foreach (InteractionInstance instance in _interactions.Values)
        {
            await instance.OnTickAsync(state);
        }

        List<GameEntityId> toRemove = [];
        foreach ((GameEntityId sourceId, InteractionInstance instance) in _interactions)
        {
            if (instance.IsOver(state))
            {
                await instance.OnEndAsync(state);
                toRemove.Add(sourceId);
            }
        }

        foreach (GameEntityId sourceId in toRemove)
        {
            if (!_interactions.Remove(sourceId, out InteractionInstance? instance))
            {
                _logger.LogWarning("Could not remove interaction for {id}", sourceId);
                continue;
            }

            instance.Source.CurrentInteraction = null;

            await _publisher.Publish(new InteractionEnded { InteractionInstance = instance });
        }
    }

    public InteractionInstance? GetCharacterInteraction(IInteractingEntity source) => _interactions.GetValueOrDefault(source.Id);
}
