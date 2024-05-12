﻿using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using SandboxGame.Experience;

namespace SandboxGame.Jobs;

public class Herbalist
{
    public Herbalist()
    {
        Peppermint = new Item
        {
            Name = "Peppermint",
            Description = "Peppermint is known for its refreshing flavor and cooling sensation. It's commonly used to aid digestion, relieve headaches, and soothe sore muscles. "
                          + "Peppermint tea is a popular way to enjoy its benefits.",
            Weight = 1
        };

        Lavender = new Item
        {
            Name = "Lavender",
            Description =
                "Lavender is known for its calming properties and is often used in aromatherapy, herbal teas, and medicinal preparations to promote relaxation and alleviate stress.",
            Weight = 1
        };

        Ginseng = new Item
        {
            Name = "Ginseng",
            Description =
                "Ginseng is a highly valued herb in traditional medicine, known for its adaptogenic properties that help the body cope with stress and promote overall well-being. "
                + "It's often used in teas, tinctures, and supplements.",
            Weight = 1
        };

        Chamomile = new Item
        {
            Name = "Chamomile",
            Description = "Chamomile is prized for its soothing effects and is commonly used to promote relaxation, aid digestion, and relieve insomnia. "
                          + "It's often brewed into herbal teas or used in topical preparations.",
            Weight = 1
        };

        Echinacea = new Item
        {
            Name = "Echinacea",
            Description =
                "Echinacea is renowned for its immune-boosting properties and is often used to prevent or shorten the duration of colds and other respiratory infections. "
                + "It's typically consumed as a tea or taken in supplement form.",
            Weight = 1
        };

        PeppermintPlant = new StaticObject { Name = "Peppermint Plant" };
        LavenderPlant = new StaticObject { Name = "Lavender Plant" };
        GinsengPlant = new StaticObject { Name = "Ginseng Plant" };
        ChamomilePlant = new StaticObject { Name = "Chamomile Plant" };
        EchinaceaPlant = new StaticObject { Name = "Echinacea Plant" };

        GatherPeppermint = new JobHarvest
        {
            Name = "gather-peppermint",
            Level = 1,
            HarvestDuration = 2,
            Targets = [PeppermintPlant],
            Items = [new ItemStack(Peppermint, 1)],
            Experience = 1
        };

        GatherLavender = new JobHarvest
        {
            Name = "gather-lavender",
            Level = 10,
            HarvestDuration = 2,
            Targets = [LavenderPlant],
            Items = [new ItemStack(Lavender, 1)],
            Experience = 5
        };

        GatherGinseng = new JobHarvest
        {
            Name = "gather-ginseng",
            Level = 20,
            HarvestDuration = 2,
            Targets = [GinsengPlant],
            Items = [new ItemStack(Ginseng, 1)],
            Experience = 15
        };

        GatherChamomile = new JobHarvest
        {
            Name = "gather-chamomile",
            Level = 30,
            HarvestDuration = 2,
            Targets = [ChamomilePlant],
            Items = [new ItemStack(Chamomile, 1)],
            Experience = 30
        };

        GatherEchinacea = new JobHarvest
        {
            Name = "gather-echinacea",
            Level = 40,
            HarvestDuration = 2,
            Targets = [EchinaceaPlant],
            Items = [new ItemStack(Echinacea, 1)],
            Experience = 50
        };

        Job = new Job
        {
            Name = "herbalist", Description = "Herbalists gather plants.", Innate = true, LevelCaps = KnownExperienceFormulas.JobLevelCaps1To50,
            Harvests = [GatherLavender, GatherGinseng, GatherChamomile, GatherEchinacea, GatherPeppermint]
        };
    }


    public Job Job { get; }
    public Item Lavender { get; }
    public Item Ginseng { get; }
    public Item Chamomile { get; set; }
    public Item Echinacea { get; set; }
    public Item Peppermint { get; set; }
    public StaticObject PeppermintPlant { get; }
    public StaticObject LavenderPlant { get; }
    public StaticObject GinsengPlant { get; }
    public StaticObject ChamomilePlant { get; }
    public StaticObject EchinaceaPlant { get; }
    public JobHarvest GatherLavender { get; }
    public JobHarvest GatherGinseng { get; }
    public JobHarvest GatherChamomile { get; }
    public JobHarvest GatherEchinacea { get; }
    public JobHarvest GatherPeppermint { get; }
}