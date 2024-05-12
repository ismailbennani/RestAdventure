using System.Collections.Concurrent;
using RestAdventure.Core.Actions;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.History;
using RestAdventure.Core.Serialization.Combats;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Core.Serialization.Players;
using RestAdventure.Kernel.Security;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Serialization;

public class GameSnapshot
{
    public required GameSettings Settings { get; init; }
    public required GameContent Content { get; init; }

    public required long Tick { get; init; }
    public required IReadOnlyDictionary<UserId, PlayerSnapshot> Players { get; init; }
    public required IReadOnlyDictionary<GameEntityId, GameEntitySnapshot> Entities { get; init; }
    public required IReadOnlyCollection<HistoryEntry> History { get; init; }
    public required IReadOnlyDictionary<CombatInstanceId, CombatInstanceSnapshot> Combats { get; init; }
    public required GameActionsSnapshot Actions { get; init; }

    public static GameSnapshot Take(Game game) =>
        new()
        {
            Settings = game.Settings,
            Content = game.Content,
            Tick = game.Tick,
            Players = game.Players.All.ToDictionary(p => p.User.Id, PlayerSnapshot.Take),
            Entities = game.Entities.ToDictionary(e => e.Id, GameEntitySnapshot.Take),
            History = game.History.All.ToArray(),
            Combats = game.Combats.ToDictionary(c => c.Id, CombatInstanceSnapshot.Take),
            Actions = GameActionsSnapshot.Take(game.Actions)
        };
}

public class GameActionsSnapshot
{
    readonly IReadOnlyCollection<IActionsProvider> _actionProviders;
    readonly ConcurrentDictionary<CharacterId, IReadOnlyCollection<Action>> _availableActions = new();

    public GameActionsSnapshot(IReadOnlyCollection<IActionsProvider> actionProviders)
    {
        _actionProviders = actionProviders;
    }

    public required IReadOnlyDictionary<CharacterId, Action> OngoingAction { get; init; }
    public required IReadOnlyDictionary<CharacterId, Action> QueuedAction { get; init; }

    public IReadOnlyCollection<Action> GetAvailableActions(GameSnapshot snapshot, CharacterSnapshot character) =>
        _availableActions.GetOrAdd(character.Id, _ => _actionProviders.SelectMany(p => p.GetActions(snapshot, character)).ToArray());

    public static GameActionsSnapshot Take(GameActions game) =>
        new(game.ActionProviders)
        {
            OngoingAction = game.Ongoing.ToDictionary(kv => kv.Key, kv => kv.Value),
            QueuedAction = game.Queued.ToDictionary(kv => kv.Key, kv => kv.Value)
        };
}
