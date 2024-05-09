namespace RestAdventure.Core.Simulation;

public abstract class SimulationWorkProvider
{
    public virtual IEnumerable<GameSimulation.Work> PreTick() => Enumerable.Empty<GameSimulation.Work>();
    public virtual IEnumerable<GameSimulation.Work> Early() => Enumerable.Empty<GameSimulation.Work>();
    public virtual IEnumerable<GameSimulation.Work> Tick() => Enumerable.Empty<GameSimulation.Work>();
    public virtual IEnumerable<GameSimulation.Work> Late() => Enumerable.Empty<GameSimulation.Work>();
    public virtual IEnumerable<GameSimulation.Work> PostTick() => Enumerable.Empty<GameSimulation.Work>();
}
