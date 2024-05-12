using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Items;

namespace SandboxGame.Monsters;

public class Rattlings
{
    public Rattlings()
    {
        Fur = new Item
        {
            Name = "Fur - Rattlings",
            Weight = 1
        };

        Skull = new Item
        {
            Name = "Skull - Rattlings",
            Weight = 5
        };

        PhantomTail = new Item
        {
            Name = "Phantom tail - Rattlings",
            Weight = 1
        };

        SpectralCheese = new Item
        {
            Name = "Spectral cheese - Rattlings",
            Weight = 1
        };

        Handkerchief = new Item
        {
            Name = "Handkerchief - Rattlings",
            Weight = 1
        };

        SwordBlade = new Item
        {
            Name = "Sword blade - Rattlings",
            Weight = 1
        };

        DirtyShirt = new Item
        {
            Name = "Dirty shirt - Rattlings",
            Weight = 1
        };

        BellyFat = new Item
        {
            Name = "Belly fat - Rattlings",
            Weight = 1
        };

        SparklingWand = new Item
        {
            Name = "Sparkling wand - Rattlings",
            Weight = 1
        };

        AncientTomeOfMagic = new Item
        {
            Name = "Ancient tome of magic - Rattlings",
            Weight = 1
        };

        Family = new MonsterFamily
        {
            Name = "Rattlings",
            Description = "A Rattling is not your average rodent; they're the mischievous troublemakers of the underground world, with quick wits and even quicker paws. "
                          + "Their tiny stature belies their big personalities, as they scurry through the shadows with a swagger in their step and a twinkle in their whiskers. "
                          + "Always ready for a challenge and never one to back down from an adventure, a Rattling will charm you with their sass and outsmart you with their cunning. "
                          + "They're the pint-sized rebels of the Rat Kingdom, ruling the sewers with a playful smirk and a tail flick of defiance.",
            Items = [new ItemStack(Fur, 1), new ItemStack(Skull, 1)]
        };

        Fantorat = new MonsterSpecies
        {
            Family = Family,
            Name = "Fantorat",
            Description = "These spectral rodents haunt the shadowy corners of ancient catacombs. "
                          + "Their ethereal forms flitting through the darkness as they search for lost treasures whispered about in ghostly legends.",
            Items = [new ItemStack(PhantomTail, 1), new ItemStack(SpectralCheese, 1)],
            Experience = 1,
            Health = 2,
            Speed = 180,
            Attack = 1
        };

        Rapierat = new MonsterSpecies
        {
            Family = Family,
            Name = "Rapierat",
            Description = "Masters of finesse and precision, Rapierats are skilled duelists who excel in the art of swordplay. "
                          + "With their trusty rapier in hand, they move with elegance and grace, striking swiftly and decisively in battle.",
            Items = [new ItemStack(Handkerchief, 1), new ItemStack(SwordBlade, 1)],
            Experience = 2,
            Health = 10,
            Speed = 140,
            Attack = 6
        };

        Biggaud = new MonsterSpecies
        {
            Family = Family,
            Name = "Biggaud",
            Description = "Towering over their counterparts, Biggauds are the brutes of the rattlings. "
                          + "With their immense strength and sturdy build, they serve as formidable defenders, capable of enduring even the fiercest of attacks.",
            Items = [new ItemStack(DirtyShirt, 1), new ItemStack(BellyFat, 1)],
            Experience = 2,
            Health = 20,
            Speed = 120,
            Attack = 2
        };

        Melurat = new MonsterSpecies
        {
            Family = Family,
            Name = "Melurat",
            Description = "Gifted with the ability to harness magic, Melurats are esteemed members of the rattling community. "
                          + "Though physically frail, their mastery of spells and enchantments makes them invaluable allies in times of need, wielding arcane forces to protect their kin.",
            Items = [new ItemStack(SparklingWand, 1), new ItemStack(AncientTomeOfMagic, 1)],
            Experience = 3,
            Health = 6,
            Speed = 100,
            Attack = 2
        };
    }


    public MonsterFamily Family;

    public Item Fur { get; }
    public Item Skull { get; }
    public Item SpectralCheese { get; }
    public Item PhantomTail { get; }
    public Item Handkerchief { get; }
    public Item SwordBlade { get; }
    public Item DirtyShirt { get; }
    public Item BellyFat { get; }
    public Item SparklingWand { get; }
    public Item AncientTomeOfMagic { get; }

    public MonsterSpecies Fantorat;
    public MonsterSpecies Rapierat;
    public MonsterSpecies Biggaud;
    public MonsterSpecies Melurat;
}
