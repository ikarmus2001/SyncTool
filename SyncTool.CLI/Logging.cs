using Serilog;

namespace SyncTool.CLI;

internal static class Logging
{
    public static ILogger CreateNewFileLogger() 
        => new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    Path.Combine(
                        Environment.CurrentDirectory,
                        $"{nameof(SyncTool)}.{nameof(SyncTool.CLI)}_log.txt"
                    )
                )
            .CreateLogger();
}
