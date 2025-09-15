using SyncTool.Lib;

namespace SyncTool.Worker;

public class Worker : BackgroundService
{
    public Guid workerId { get; init; }

    private readonly ILogger<Worker> _logger;
    private readonly SyncWorker _sync;
    private readonly PeriodicTimer _timer;
    internal readonly CancellationTokenSource _cancelTokenSource;

    public Worker(ILogger<Worker> logger, SyncWorker sync, CancellationTokenSource cancelTokenSource)
    {
        _logger = logger;
        _sync = sync;
        _cancelTokenSource = cancelTokenSource;
        _timer = new(_sync.Period);
        workerId = Guid.NewGuid();

        _logger.LogInformation($"Worker {workerId} created for syncing '{_sync.SourcePath}' to '{_sync.TargetPath}' every {_sync.Period}");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            using (_logger.BeginScope("SyncScope"))
            {
                _sync.SyncFiles();
            }
            await _timer.WaitForNextTickAsync(stoppingToken);
        }
        while (!stoppingToken.IsCancellationRequested);
    }
}
