using RestAdventure.Core.Characters;

namespace RestAdventure.Core.Conditions.Characters;

public interface ICharacterCondition
{
    public bool Evaluate(Character character);
}
