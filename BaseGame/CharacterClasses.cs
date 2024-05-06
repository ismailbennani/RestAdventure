using System.Reflection;
using RestAdventure.Core.Characters;

namespace BaseGame;

public class CharacterClasses
{
    public CharacterClasses()
    {
        Knight = new CharacterClass
        {
            Name = "Knight",
            Description = "The Knight, the one who carries the weight of the world on their shoulders, or at least all the blows from enemies. "
                          + "Sturdy as a fortress, tough as nails, and about as subtle as a charging rhino.",
            LevelCaps = [2, 5, 10]
        };

        Mage = new CharacterClass
        {
            Name = "Mage",
            Description = "Behold, the Mage! Their power rivals that of the gods themselves, yet one swift breeze could knock them over. "
                          + "Fragile as a butterfly's wing, but with a mind as sharp as a wizard's hat.",
            LevelCaps = [2, 5, 10]
        };

        Scout = new CharacterClass
        {
            Name = "Scout",
            Description = "Presenting the Scout, the embodiment of speed and stealth, with a touch of sass thrown in for good measure. "
                          + "Quick as lightning and sneaky as a pickpocket in a crowded market, they dance through danger with all the grace of a cat on a hot tin roof. "
                          + "Sure, they might not bench-press boulders, but who needs muscles when you can outsmart your foes before they even know you're there? Fragile? Maybe. "
                          + "But they'll have you eating their dust faster than you can say 'gotcha'.",
            LevelCaps = [2, 5, 10]
        };

        Dealer = new CharacterClass
        {
            Name = "Dealer",
            Description = "Ah, the Dealer, not one for brawls but they'll haggle the pants off a troll. Their weapon of choice? The art of the deal. "
                          + "While others swing swords, they wield contracts and coin purses with finesse. Just don't expect them to throw down in a fistfight unless it's over prices.",
            LevelCaps = [2, 5, 10]
        };
    }

    public CharacterClass Knight { get; }
    public CharacterClass Mage { get; }
    public CharacterClass Scout { get; }
    public CharacterClass Dealer { get; }

    public IEnumerable<CharacterClass> All =>
        typeof(CharacterClass).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.PropertyType == typeof(CharacterClass))
            .Select(p => p.GetValue(this))
            .OfType<CharacterClass>();
}
