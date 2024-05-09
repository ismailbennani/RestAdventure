using RestAdventure.Core.Actions.Notifications;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Actions;

public record ActionId(Guid Guid);

public abstract class Action : IEquatable<Action>
{
    protected Action(string name)
    {
        Name = name;
    }

    public ActionId Id { get; } = new(Guid.NewGuid());

    public string Name { get; }
    public long StartTick { get; private set; }
    public bool Started { get; private set; }
    public bool Over { get; protected set; }
    public bool Ended { get; private set; }

    /// <summary>
    ///     Can the action be performed
    /// </summary>
    public Maybe CanPerform(GameState state, Character character)
    {
        if (character.Busy)
        {
            return "Character is busy";
        }

        return CanPerformInternal(state, character);
    }

    /// <inheritdoc cref="CanPerform" />
    protected virtual Maybe CanPerformInternal(GameState state, Character character) => true;

    public async Task StartAsync(GameState state, Character character)
    {
        StartTick = state.Tick;
        Started = true;
        character.Busy = true;

        await OnStartAsync(state, character);

        await state.Publisher.Publish(new ActionStarted { Character = character, Action = this });
    }

    public async Task TickAsync(GameState state, Character character) => await OnTickAsync(state, character);

    public async Task EndAsync(GameState state, Character character)
    {
        Ended = true;
        character.Busy = false;

        await OnEndAsync(state, character);

        await state.Publisher.Publish(new ActionEnded { Character = character, Action = this });
    }

    /// <summary>
    ///     Called on the first tick, even if <see cref="Over" /> is true. It is guaranteed to be called exactly once per interaction.
    ///     When this hook is called on the same tick as <see cref="OnStartAsync" /> or <see cref="OnTickAsync" />, it is always called first.
    /// </summary>
    protected virtual Task OnStartAsync(GameState state, Character character) => Task.CompletedTask;

    /// <summary>
    ///     Called once per tick.
    ///     This hook is called on every tick. It is guaranteed to be called at least once (on the first tick).
    ///     When this hook is called on the same tick as <see cref="OnStartAsync" />, it will be called AFTER it.
    ///     When this hook is called on the same tick as <see cref="OnEndAsync" />, it will be called BEFORE it.
    /// </summary>
    protected virtual Task OnTickAsync(GameState state, Character character) => Task.CompletedTask;

    /// <summary>
    ///     Called on the last tick, even if it is the first one. It is guaranteed to be called exactly once per interaction.
    ///     The last tick is the first tick after the start of the interaction where <see cref="Over" /> is true.
    ///     This means that this hook MIGHT be called on the same tick as <see cref="OnStartAsync" />, if <see cref="Over" /> is true on the first tick.
    ///     When this hook is called on the same tick as <see cref="OnStartAsync" /> or <see cref="OnTickAsync" />, it is always called last.
    /// </summary>
    protected virtual Task OnEndAsync(GameState state, Character character) => Task.CompletedTask;

    public bool Equals(Action? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj.GetType() != GetType())
        {
            return false;
        }
        return Equals((Action)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Action? left, Action? right) => Equals(left, right);

    public static bool operator !=(Action? left, Action? right) => !Equals(left, right);
}
