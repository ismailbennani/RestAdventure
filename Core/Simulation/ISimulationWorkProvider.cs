namespace RestAdventure.Core.Simulation;

public abstract class SimulationWorkProvider
{
    /// <summary>
    ///     Called once when a new game is created
    /// </summary>
    public virtual IEnumerable<GameSimulation.Work> Initialization() => Enumerable.Empty<GameSimulation.Work>();

    /// <summary>
    ///     Called once per tick, before the other tick events
    /// </summary>
    public virtual IEnumerable<GameSimulation.Work> PreTick() => Enumerable.Empty<GameSimulation.Work>();

    /// <summary>
    ///     Called once per tick, after <see cref="PreTick" />
    /// </summary>
    public virtual IEnumerable<GameSimulation.Work> Early() => Enumerable.Empty<GameSimulation.Work>();

    /// <summary>
    ///     Called once per tick, after <see cref="Early" />
    /// </summary>
    public virtual IEnumerable<GameSimulation.Work> Tick() => Enumerable.Empty<GameSimulation.Work>();

    /// <summary>
    ///     Called once per tick, after <see cref="Tick" />
    /// </summary>
    public virtual IEnumerable<GameSimulation.Work> Late() => Enumerable.Empty<GameSimulation.Work>();

    /// <summary>
    ///     Called once per tick, after <see cref="Late" />
    /// </summary>
    public virtual IEnumerable<GameSimulation.Work> PostTick() => Enumerable.Empty<GameSimulation.Work>();
}
