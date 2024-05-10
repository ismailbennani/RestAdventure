using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Maps.Locations;
using SandboxGame.Generation;

namespace SandboxGame.Characters;

public class CharacterClasses
{
    public CharacterClasses(GeneratedMaps maps)
    {
        Location startLocation = maps.Locations.MinBy(l => Math.Abs(l.PositionX) + Math.Abs(l.PositionY)) ?? throw new InvalidOperationException("Map is empty");

        Knight = new CharacterClass
        {
            Name = "Knight",
            Description = "The Knight, the one who carries the weight of the world on their shoulders, or at least all the blows from enemies. "
                          + "Sturdy as a fortress, tough as nails, and about as subtle as a charging rhino.",
            StartLocation = startLocation,
            Health = 100,
            Speed = 100,
            Attack = 10,
            LevelCaps = [2, 5, 10]
        };

        Mage = new CharacterClass
        {
            Name = "Mage",
            Description = "Behold, the Mage! Their power rivals that of the gods themselves, yet one swift breeze could knock them over. "
                          + "Fragile as a butterfly's wing, but with a mind as sharp as a wizard's hat.",
            StartLocation = startLocation,
            Health = 100,
            Speed = 100,
            Attack = 10,
            LevelCaps = [2, 5, 10]
        };

        Scout = new CharacterClass
        {
            Name = "Scout",
            Description = "Presenting the Scout, the embodiment of speed and stealth, with a touch of sass thrown in for good measure. "
                          + "Quick as lightning and sneaky as a pickpocket in a crowded market, they dance through danger with all the grace of a cat on a hot tin roof. "
                          + "Sure, they might not bench-press boulders, but who needs muscles when you can outsmart your foes before they even know you're there? Fragile? Maybe. "
                          + "But they'll have you eating their dust faster than you can say 'gotcha'.",
            StartLocation = startLocation,
            Health = 100,
            Speed = 100,
            Attack = 10,
            LevelCaps = [2, 5, 10]
        };

        Dealer = new CharacterClass
        {
            Name = "Dealer",
            Description = "Ah, the Dealer, not one for brawls but they'll haggle the pants off a troll. Their weapon of choice? The art of the deal. "
                          + "While others swing swords, they wield contracts and coin purses with finesse. Just don't expect them to throw down in a fistfight unless it's over prices.",
            StartLocation = startLocation,
            Health = 100,
            Speed = 100,
            Attack = 10,
            LevelCaps = [2, 5, 10]
        };
    }

    public CharacterClass Knight { get; }
    public CharacterClass Mage { get; }
    public CharacterClass Scout { get; }
    public CharacterClass Dealer { get; }
}
