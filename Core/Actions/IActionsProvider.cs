using RestAdventure.Core.Serialization;
using RestAdventure.Core.Serialization.Entities;

namespace RestAdventure.Core.Actions;

public interface IActionsProvider
{
    IEnumerable<Action> GetActions(GameSnapshot state, CharacterSnapshot character);
}
