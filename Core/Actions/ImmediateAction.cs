using RestAdventure.Core.Entities.Characters;

namespace RestAdventure.Core.Actions;

public abstract class ImmediateAction : Action
{
    protected ImmediateAction(string name) : base(name)
    {
    }

    public override bool IsOver(GameState state, Character character) => true;

    protected override sealed Task OnStartAsync(GameState state, Character character) => Task.CompletedTask;
    protected override sealed Task OnTickAsync(GameState state, Character character) => PerformAsync(state, character);
    protected override sealed Task OnEndAsync(GameState state, Character character) => Task.CompletedTask;

    protected abstract Task PerformAsync(GameState state, Character character);
}
