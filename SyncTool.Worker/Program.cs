namespace SyncTool.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services
            .AddSingleton<WorkerManager>()
            .AddHostedService<NamedPipeListener>();

        var host = builder.Build();
        host.Run();
    }
}