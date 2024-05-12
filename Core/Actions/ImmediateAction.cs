using RestAdventure.Core.Entities.Characters;

namespace RestAdventure.Core.Actions;

public abstract class ImmediateAction : Action
{
    protected ImmediateAction(string name) : base(name)
    {
    }

    public override bool IsOver(Game state, Character character) => true;

    protected override sealed Task OnStartAsync(Game state, Character character) => Task.CompletedTask;
    protected override sealed Task OnTickAsync(Game state, Character character) => PerformAsync(state, character);
    protected override sealed Task OnEndAsync(Game state, Character character) => Task.CompletedTask;

    protected abstract Task PerformAsync(Game state, Character character);
}
