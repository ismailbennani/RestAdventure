using RestAdventure.Core.Characters;

namespace RestAdventure.Core.Actions.Providers;

public interface IActionProvider
{
    IEnumerable<Action> GetActions(Character character);
}
