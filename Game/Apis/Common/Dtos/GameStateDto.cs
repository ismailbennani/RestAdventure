using System.ComponentModel.DataAnnotations;
using RestAdventure.Core;
using RestAdventure.Game.Apis.GameApi.Services.Game;

namespace RestAdventure.Game.Apis.Common.Dtos;

/// <summary>
///     Game state
/// </summary>
public class GameStateDto
{
    /// <summary>
    ///     The current game tick
    /// </summary>
    [Required]
    public required long Tick { get; init; }

    /// <summary>
    ///     Is the next tick being computed.
    ///     In that case NextTickDate refers to the old tick's next tick date, which means that it is probably in the past.
    /// </summary>
    [Required]
    public required bool IsComputingNextTick { get; init; }

    /// <summary>
    ///     Is the game paused?
    /// </summary>
    [Required]
    public required bool Paused { get; init; }

    /// <summary>
    ///     If the game is started, the date at which last tick has been computed
    /// </summary>
    public required DateTime? LastTickDate { get; init; }

    /// <summary>
    ///     If the game is not paused, the date at which next tick will be computed
    /// </summary>
    public required DateTime? NextTickDate { get; init; }
}

static class GameStateMappingExtensions
{
    public static GameStateDto ToDto(this GameState state, GameScheduler scheduler) =>
        new()
        {
            Tick = state.Tick,
            IsComputingNextTick = scheduler.IsComputingNextTick,
            Paused = scheduler.Paused,
            LastTickDate = scheduler.LastStepDate,
            NextTickDate = scheduler.Paused ? null : scheduler.NextStepDate
        };
}
