using RestAdventure.Core.Characters;

namespace RestAdventure.Core.Gameplay.Interactions;

public record InteractionInstanceId(Guid Guid);

public abstract class InteractionInstance
{
    protected InteractionInstance(Character character, Interaction interaction, IGameEntityWithInteractions subject)
    {
        Character = character;
        Interaction = interaction;
        Subject = subject;
    }

    public InteractionInstanceId Id { get; } = new(Guid.NewGuid());

    /// <summary>
    ///     The interaction being performed
    /// </summary>
    public Interaction Interaction { get; }

    /// <summary>
    ///     The character performing the operation
    /// </summary>
    public Character Character { get; }

    /// <summary>
    ///     The subject of the interaction
    /// </summary>
    public IGameEntityWithInteractions Subject { get; }

    public abstract bool IsOver(GameState state);

    /// <summary>
    ///     This hook is called on the first tick, even if <see cref="IsOver" /> returns true. It is guaranteed to be called exactly once per interaction.
    ///     When this hook is called on the same tick as <see cref="OnStartAsync" /> or <see cref="OnTickAsync" />, it is always called first.
    /// </summary>
    public virtual Task OnStartAsync(GameState state) => Task.CompletedTask;

    /// <summary>
    ///     Called once per tick between the first and the last one.
    ///     This hook is called on every tick. It is guaranteed to be called at least once (on the first tick).
    ///     When this hook is called on the same tick as <see cref="OnStartAsync" />, it will be called AFTER it.
    ///     When this hook is called on the same tick as <see cref="OnEndAsync" />, it will be called BEFORE it.
    /// </summary>
    public virtual Task OnTickAsync(GameState state) => Task.CompletedTask;

    /// <summary>
    ///     This hook is called on the last tick, even if it is the first one. It is guaranteed to be called exactly once per interaction.
    ///     The last tick is the first tick after the start of the interaction where <see cref="IsOver" /> returns true.
    ///     This means that this hook MIGHT be called on the same tick as <see cref="OnStartAsync" />, if <see cref="IsOver" /> returns true on the first tick.
    ///     When this hook is called on the same tick as <see cref="OnStartAsync" /> or <see cref="OnTickAsync" />, it is always called last.
    /// </summary>
    public virtual Task OnEndAsync(GameState state) => Task.CompletedTask;
}
