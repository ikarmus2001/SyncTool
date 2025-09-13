using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace SyncTool.xUnit;

internal class Logging
{
    private ILoggerFactory LoggerFactory;
    public ILogger Logger;

    public Logging(ITestOutputHelper testOutputHelper)
    {
        LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddXUnit(testOutputHelper)
                .SetMinimumLevel(LogLevel.Debug);
        });
        Logger = CreateLogger();
    }

    private ILogger CreateLogger()
        => LoggerFactory.CreateLogger(nameof(xUnit));
}
