using MediatR;
using RestAdventure.Core.Simulation.Notifications;

namespace RestAdventure.Core.Simulation;

public class GameSimulation
{
    readonly IPublisher _publisher;

    public GameSimulation(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task TickAsync(GameState state)
    {
        state.Tick++;

        await state.Actions.StartQueuedActionsAsync(state);

        await state.Combats.ResolveCombatsAsync(state);

        await state.Actions.TickOngoingActionsAsync(state);
        await state.Actions.RemoveFinishedActionsAsync(state);
        state.Actions.OnTickEnd(state);

        await _publisher.Publish(new GameTick { GameState = state });
    }
}
