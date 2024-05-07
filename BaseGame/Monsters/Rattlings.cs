using RestAdventure.Core.Monsters;

namespace BaseGame.Monsters;

public class Rattlings
{
    public Rattlings()
    {
        Family = new MonsterFamily
        {
            Name = "Rattlings",
            Description = "A Rattling is not your average rodent; they're the mischievous troublemakers of the underground world, with quick wits and even quicker paws. "
                          + "Their tiny stature belies their big personalities, as they scurry through the shadows with a swagger in their step and a twinkle in their whiskers. "
                          + "Always ready for a challenge and never one to back down from an adventure, a Rattling will charm you with their sass and outsmart you with their cunning. "
                          + "They're the pint-sized rebels of the Rat Kingdom, ruling the sewers with a playful smirk and a tail flick of defiance."
        };

        PetitPaw = new MonsterSpecies
        {
            Family = Family,
            Name = "Petit Paw",
            Description = "",
            Health = 2,
            Speed = 180,
            Attack = 1
        };

        Rapierat = new MonsterSpecies
        {
            Family = Family,
            Name = "Rapierat",
            Description = "",
            Health = 5,
            Speed = 100,
            Attack = 6
        };

        Biggaud = new MonsterSpecies
        {
            Family = Family,
            Name = "Biggaud",
            Description = "",
            Health = 10,
            Speed = 100,
            Attack = 2
        };

        Sorcerat = new MonsterSpecies
        {
            Family = Family,
            Name = "Sorcerat",
            Description = "",
            Health = 10,
            Speed = 100,
            Attack = 2
        };

        Species = [PetitPaw, Rapierat, Biggaud, Sorcerat];
    }

    public MonsterFamily Family;
    public IReadOnlyCollection<MonsterSpecies> Species { get; }

    public MonsterSpecies PetitPaw;
    public MonsterSpecies Rapierat;
    public MonsterSpecies Biggaud;
    public MonsterSpecies Sorcerat;
}
