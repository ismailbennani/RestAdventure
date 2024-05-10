using RestAdventure.Core.Entities;
using RestAdventure.Core.Simulation;

namespace RestAdventure.Core.Spawners;

public class SpawnerSimulationWorkProvider : SimulationWorkProvider
{
    readonly GameState _state;

    public SpawnerSimulationWorkProvider(GameState state)
    {
        _state = state;
    }

    public override IEnumerable<GameSimulation.Work> Initialization()
    {
        foreach (Spawner spawner in _state.Content.Maps.Spawners)
        {
            yield return new GameSimulation.Work(spawner.Id.Guid, () => SpawnAsync(spawner, s => s.GetInitialEntities(_state)));
        }
    }

    public override IEnumerable<GameSimulation.Work> Late()
    {
        foreach (Spawner spawner in _state.Content.Maps.Spawners)
        {
            yield return new GameSimulation.Work(spawner.Id.Guid, () => SpawnAsync(spawner, s => s.GetEntitiesToSpawn(_state)));
        }
    }

    async Task SpawnAsync(Spawner spawner, Func<Spawner, IEnumerable<GameEntity>> getEntities)
    {
        IEnumerable<GameEntity> toSpawn = getEntities(spawner);

        foreach (GameEntity entity in toSpawn)
        {
            await _state.Entities.AddAsync(entity);
        }
    }
}
