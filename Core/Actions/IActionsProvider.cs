using RestAdventure.Core.Entities.Characters;

namespace RestAdventure.Core.Actions;

public interface IActionsProvider
{
    IEnumerable<Action> GetActions(GameState state, Character character);
}
