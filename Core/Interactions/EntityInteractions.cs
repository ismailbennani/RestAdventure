namespace RestAdventure.Core.Interactions;

public class EntityInteractions
{
    readonly Dictionary<InteractionId, Interaction> _interactions;

    public EntityInteractions(params Interaction[] interactions)
    {
        _interactions = interactions.ToDictionary(i => i.Id, i => i);
    }

    public IEnumerable<Interaction> All => _interactions.Values;
    public Interaction? Get(InteractionId interactionId) => _interactions.GetValueOrDefault(interactionId);
}
