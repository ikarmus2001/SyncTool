using SyncTool.Lib;
using SyncTool.Lib.Communication;
using System.IO.Pipes;
using System.Text.Json;

namespace SyncTool.Worker;

internal class NamedPipeListener(WorkerManager manager) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"PID: {Environment.ProcessId}");
        while (!stoppingToken.IsCancellationRequested)
        {
            using var server = new System.IO.Pipes.NamedPipeServerStream(
                Constants.PipeName,
                System.IO.Pipes.PipeDirection.InOut
            );

            await server.WaitForConnectionAsync(stoppingToken);
            await HandleCommunicationAsync(server, stoppingToken);
        }
    }

    private async Task HandleCommunicationAsync(PipeStream server, CancellationToken stoppingToken)
    {
        SyncWorker? workerConfig;
        using var reader = new StreamReader(server, leaveOpen: true);
        using var writer = new StreamWriter(server) { AutoFlush = true };

        var payload = await reader.ReadLineAsync(stoppingToken);
        try
        {
            workerConfig = JsonSerializer.Deserialize<SyncWorker>(payload);
        }
        catch (JsonException jex)
        {
            return;
        }
        var createdWorkerId = await manager.AddWorkerAsync(workerConfig);

        var response = JsonSerializer.Serialize(new CreateWorkerResponse(createdWorkerId));
        await writer.WriteAsync(response);
    }
}