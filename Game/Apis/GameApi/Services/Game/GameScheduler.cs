using Microsoft.Extensions.Options;
using RestAdventure.Core;
using RestAdventure.Game.Settings;

namespace RestAdventure.Game.Apis.GameApi.Services.Game;

/// <summary>
///     Game scheduler
/// </summary>
public class GameScheduler : IDisposable
{
    readonly GameService _gameService;
    readonly IOptions<ServerSettings> _serverSettings;
    readonly ILogger<GameScheduler> _logger;

    CancellationTokenSource? _mainLoopCancellationSource;

    /// <summary>
    /// </summary>
    public GameScheduler(GameService gameService, IOptions<ServerSettings> serverSettings, ILogger<GameScheduler> logger)
    {
        _gameService = gameService;
        _serverSettings = serverSettings;
        _logger = logger;
    }

    /// <summary>
    ///     The last tick date
    /// </summary>
    public DateTime? LastStepDate { get; private set; }

    /// <summary>
    ///     The next tick date
    /// </summary>
    public DateTime? NextStepDate { get; private set; }

    /// <summary>
    ///     Is the simulation paused
    /// </summary>
    public bool Paused { get; private set; } = true;

    /// <summary>
    ///     Start the simulation
    /// </summary>
    public void Start(TimeSpan? firstTickDelay = null)
    {
        if (_mainLoopCancellationSource != null)
        {
            throw new InvalidOperationException("Scheduler is already started.");
        }

        Paused = false;
        NextStepDate = DateTime.Now;

        _mainLoopCancellationSource = new CancellationTokenSource();
        CancellationToken token = _mainLoopCancellationSource.Token;

        if (firstTickDelay.HasValue)
        {
            _logger.LogInformation("Game simulation is starting (delay: {seconds}s).", firstTickDelay.Value.TotalSeconds);
        }
        else
        {
            _logger.LogInformation("Game simulation is starting.");
        }

        // start main loop and forget about it
        Task _ = MainLoopAsync(firstTickDelay, token);
    }

    /// <summary>
    ///     Stop the simulation
    /// </summary>
    public void Stop()
    {
        if (_mainLoopCancellationSource == null)
        {
            throw new InvalidOperationException("Scheduler is already stopped.");
        }

        Paused = true;
        NextStepDate = null;

        _logger.LogInformation("Game simulation is stopping.");

        _mainLoopCancellationSource.Cancel();
        _mainLoopCancellationSource.Dispose();
        _mainLoopCancellationSource = null;
    }

    /// <summary>
    ///     Tick the simulation now
    /// </summary>
    public async Task TickNowAsync()
    {
        try
        {
            await _gameService.TickAsync();
        }
        catch (Exception exn)
        {
            _logger.LogError(exn, "An error occured in {method}.", nameof(_gameService.TickAsync));
        }

        TimeSpan tickDuration = _serverSettings.Value.TickDuration;

        LastStepDate = DateTime.Now;
        NextStepDate = LastStepDate + tickDuration;
    }

    async Task MainLoopAsync(TimeSpan? firstTickDelay, CancellationToken cancellationToken = default)
    {
        if (firstTickDelay.HasValue)
        {
            await Task.Delay(firstTickDelay.Value, cancellationToken);
        }

        while (true)
        {
            if (cancellationToken.IsCancellationRequested || !NextStepDate.HasValue)
            {
                break;
            }

            DateTime now = DateTime.Now;
            if (now >= NextStepDate)
            {
                await TickNowAsync();
            }

            TimeSpan toWait = NextStepDate.Value - now;
            await Task.Delay(toWait, cancellationToken);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _mainLoopCancellationSource?.Dispose();
        GC.SuppressFinalize(this);
    }
}
