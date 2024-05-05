using MediatR;

namespace RestAdventure.Core.Simulation.Notifications;

public class GameTick : INotification
{
    public required GameState GameState { get; init; }

    public override string ToString() => $"Tick: {GameState.Tick}";
}
