using SyncTool.Lib;

namespace SyncTool.Worker;

internal class WorkerManager()
{
    private readonly List<Worker> _workers = new();
    private readonly LoggerFactory _loggerFactory = new();

    public async Task<Guid> AddWorkerAsync(SyncWorker worker)
    {
        var cancelTokenSource = new CancellationTokenSource();
        var w = new Worker(_loggerFactory.CreateLogger<Worker>(), worker, cancelTokenSource);
        await w.StartAsync(w._cancelTokenSource.Token);
        _workers.Add(w);

        return w.workerId;
    }

    public async Task<ushort> StopWorker(Guid? workerGuid)
    {
        if (workerGuid == null)
        {
            var tasks = _workers.Select(w => w._cancelTokenSource.CancelAsync()).ToArray();
            Task.WaitAll(tasks);
            _workers.Clear();
            return (ushort)tasks.Length;
        }

        var worker = _workers.FirstOrDefault(w => w.workerId == workerGuid);
        if (worker != null)
        {
            await worker.StopAsync(worker._cancelTokenSource.Token);
            _workers.Remove(worker);
            return 1;
        }
        throw new ArgumentException($"Worker {workerGuid} not found");
    }
}
