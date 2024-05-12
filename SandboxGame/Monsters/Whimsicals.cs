using RestAdventure.Core.Entities.Monsters;
using SandboxGame.Items;

namespace SandboxGame.Monsters;

public class Whimsicals
{
    public Whimsicals(GenericItemCategories categories)
    {
        Family = new MonsterFamily
        {
            Name = "Whimsicals",
            Description = "Meeting the Whimsicals is a heartwarming experience, as their adorable appearance and playful antics bring cheer to the forest. "
                          + "Despite their small stature and endearing names, these creatures possess a surprising toughness, catching unsuspecting adventurers off guard when challenged.\n",
            Items = []
        };

        PetitPaw = new MonsterSpecies
        {
            Family = Family,
            Name = "Petit Paw",
            Description = "These tiny speedsters may lack in size, but they make up for it with their lightning-fast reflexes. "
                          + "Quick, nimble, and always one step ahead, Petit Paws are the epitome of agility, darting through the shadows with grace and precision.",
            Items = [],
            Experience = 1,
            Health = 2,
            Speed = 180,
            Attack = 1
        };

        Bumblebun = new MonsterSpecies
        {
            Family = Family,
            Name = "Bumblebun",
            Description = "A small, fluffy bunny known for its clumsy hopping and endearing bumbling antics.",
            Items = [],
            Experience = 2,
            Health = 4,
            Speed = 120,
            Attack = 2
        };

        Flutterfly = new MonsterSpecies
        {
            Family = Family,
            Name = "Flutterfly",
            Description = "Delicate and dainty, these whimsical insects flit from flower to flower, spreading joy with every flutter of their wings.",
            Items = [],
            Experience = 2,
            Health = 3,
            Speed = 120,
            Attack = 2
        };

        Chirpchirp = new MonsterSpecies
        {
            Family = Family,
            Name = "Chirpchirp",
            Description =
                " Known for its cheerful demeanor and infectious laughter, the Chirpchirp brightens even the gloomiest of days with its melodious trills and joyful chirps.",
            Items = [],
            Experience = 2,
            Health = 3,
            Speed = 120,
            Attack = 2
        };
    }


    public MonsterFamily Family { get; }

    public MonsterSpecies PetitPaw { get; }
    public MonsterSpecies Bumblebun { get; }
    public MonsterSpecies Flutterfly { get; }
    public MonsterSpecies Chirpchirp { get; }
}
