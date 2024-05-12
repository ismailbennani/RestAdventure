using RestAdventure.Core.Simulation.Notifications;

namespace RestAdventure.Core.Simulation;

public class GameSimulation
{
    readonly Game _state;
    readonly IReadOnlyCollection<SimulationWorkProvider> _providers;

    public GameSimulation(Game state, IReadOnlyCollection<SimulationWorkProvider> providers)
    {
        _state = state;
        _providers = providers;
    }

    public async Task StartAsync() => await ExecuteWorkAsync(p => p.Initialization());

    public async Task TickAsync()
    {
        _state.Tick++;

        await ExecuteWorkAsync(p => p.PreTick());
        await ExecuteWorkAsync(p => p.Early());
        await ExecuteWorkAsync(p => p.Tick());
        await ExecuteWorkAsync(p => p.Late());
        await ExecuteWorkAsync(p => p.PostTick());

        await _state.Publisher.Publish(new GameTick { Game = _state });
    }

    async Task ExecuteWorkAsync(Func<SimulationWorkProvider, IEnumerable<Work>> getWork)
    {
        IOrderedEnumerable<Work> work = _providers.SelectMany(getWork).OrderBy(w => w.Id);
        foreach (Work w in work)
        {
            await w.ExecuteAsync();
        }
    }

    public class Work
    {
        readonly Func<Task> _work;

        public Work(Guid id, Func<Task> work)
        {
            _work = work;
            Id = id;
        }

        public Guid Id { get; }

        public async Task ExecuteAsync() => await _work();
    }
}
