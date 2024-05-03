﻿using System.ComponentModel.DataAnnotations;

namespace RestAdventure.Game.Apis.GameApi.Dtos;

public class GameStateDto
{
    /// <summary>
    ///     The current game tick
    /// </summary>
    [Required]
    public required long Tick { get; init; }

    /// <summary>
    ///     Is the game paused?
    /// </summary>
    [Required]
    public required bool Paused { get; init; }

    /// <summary>
    ///     If the game is started, the date at which last tick has been computed
    /// </summary>
    [Required]
    public required DateTime? LastTickDate { get; init; }

    /// <summary>
    ///     If the game is not paused, the date at which next tick will be computed
    /// </summary>
    [Required]
    public required DateTime? NextTickDate { get; init; }
}
