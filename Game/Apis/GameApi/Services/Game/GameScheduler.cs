using Microsoft.Extensions.Options;
using RestAdventure.Core;
using RestAdventure.Game.Settings;
using RestAdventure.Kernel.Persistence;
using Xtensive.Orm;

namespace RestAdventure.Game.Apis.GameApi.Services.Game;

public class GameScheduler : IDisposable
{
    readonly DomainAccessor _domainAccessor;
    readonly GameService _gameService;
    readonly IOptions<ServerSettings> _serverSettings;
    readonly ILogger<GameScheduler> _logger;

    CancellationTokenSource? _mainLoopCancellationSource;

    public GameScheduler(DomainAccessor domainAccessor, GameService gameService, IOptions<ServerSettings> serverSettings, ILogger<GameScheduler> logger)
    {
        _domainAccessor = domainAccessor;
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
    public void Start()
    {
        if (_mainLoopCancellationSource != null)
        {
            throw new InvalidOperationException("Scheduler is already started.");
        }

        Paused = false;
        NextStepDate = DateTime.Now;

        _mainLoopCancellationSource = new CancellationTokenSource();
        CancellationToken token = _mainLoopCancellationSource.Token;

        // start main loop and forget about it
        Task _ = MainLoopAsync(token);

        _logger.LogInformation("The game simulation has started.");
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

        _mainLoopCancellationSource.Cancel();
        _mainLoopCancellationSource.Dispose();
        _mainLoopCancellationSource = null;

        _logger.LogInformation("The game simulation has stopped.");
    }

    async Task MainLoopAsync(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                await using Session session = await _domainAccessor.Domain.OpenSessionAsync(cancellationToken);
                await using TransactionScope transaction = await session.OpenTransactionAsync(cancellationToken);
                using SessionScope _ = session.Activate();

                await _gameService.TickAsync();

                transaction.Complete();
            }
            catch (Exception exn)
            {
                _logger.LogError(exn, "An error occured in {method}.", nameof(_gameService.TickAsync));
            }

            TimeSpan tickDuration = _serverSettings.Value.TickDuration;

            LastStepDate = DateTime.Now;
            NextStepDate = LastStepDate + tickDuration;

            await Task.Delay(tickDuration, cancellationToken);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _mainLoopCancellationSource?.Dispose();
        GC.SuppressFinalize(this);
    }
}
