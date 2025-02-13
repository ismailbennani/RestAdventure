﻿using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Players;
using RestAdventure.Kernel.Security;

namespace RestAdventure.Game.Apis.Common.Dtos.Players;

/// <summary>
///     A player
/// </summary>
public class PlayerDto
{
    /// <summary>
    ///     The unique ID of the player
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the player
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     The API key that grants access to the Game API
    /// </summary>
    [Required]
    public required Guid ApiKey { get; init; }
}

static class PlayerRegistrationMappingExtensions
{
    public static PlayerDto ToDto(this User player) =>
        new()
        {
            Id = player.Id.Guid,
            Name = player.Name,
            ApiKey = player.ApiKey.Guid
        };

    public static PlayerDto ToDto(this Player state) =>
        new()
        {
            Id = state.User.Id.Guid,
            Name = state.User.Name,
            ApiKey = state.User.ApiKey.Guid
        };
}
