using SyncTool.Lib;

namespace SyncTool.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly SyncWorker _sync;
    private readonly PeriodicTimer _timer;

    public Worker(ILogger<Worker> logger, SyncWorker sync)
    {
        _logger = logger;
        _sync = sync;
        _timer = new(_sync.Period);
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
