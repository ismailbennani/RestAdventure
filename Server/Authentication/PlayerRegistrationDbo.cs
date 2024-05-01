﻿using Core.Players;
using Xtensive.Orm;

namespace Server.Authentication;

[HierarchyRoot]
class PlayerRegistrationDbo : Entity
{
    public PlayerRegistrationDbo(PlayerDbo player)
    {
        Player = player;
        CreationDate = DateTime.Now;
    }

    [Key]
    [Field]
    public Guid ApiKey { get; private set; }

    [Field]
    public PlayerDbo Player { get; private set; }

    [Field]
    public DateTime CreationDate { get; private set; }
}
