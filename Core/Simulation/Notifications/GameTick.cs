using MediatR;

namespace RestAdventure.Core.Simulation.Notifications;

public class GameTick : INotification
{
    public required Game Game { get; init; }

    public override string ToString() => $"Tick: {Game.Tick}";
}
