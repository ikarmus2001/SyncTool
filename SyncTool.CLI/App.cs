using Cronos;
using SyncTool.Lib;
using System.Diagnostics;
using Command = ConsoleAppFramework.CommandAttribute;


namespace SyncTool.CLI;

internal class App
{
    Serilog.ILogger logger = Logging.CreateNewFileLogger();


    /// <summary>
    ///     Start a sync operation on a schedule
    /// </summary>
    /// <param name="source">
    ///     Path to the directory, which will be the basepoint for syncing
    /// </param>
    /// <param name="replica">
    ///     Target directory, where files will be synced to
    /// </param>
    /// <param name="cron">
    ///     Cron expression for scheduling the sync operation
    ///     Every two minutes by default
    /// </param>
    /// <param name="createReplicaDir">
    ///     Set to false to disable auto-creation of the replica directory
    /// </param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    [Command(CommandNames.sync)]
    public void Sync(string source, string replica, string cron = "*/2 * * * *", bool createReplicaDir = true)
    {
        logger.Information($"{CommandNames.sync} invoked");
        var sDirInfo = new DirectoryInfo(source);  // validate source dir
        var rDirInfo = new DirectoryInfo(replica);
        if (!sDirInfo.Exists) // && rDirInfo.Exists == false)
        {
#if DEBUG
            sDirInfo.Create();
            logger.Warning($"Source directory '{source}' did not exist, but was auto-created in DEBUG mode.");
#else
            throw new ArgumentException($"Source directory '{source}' does not exist");
#endif
        }

        if (rDirInfo.Exists == false)
        {
            if (!createReplicaDir)
            {
                throw new DirectoryNotFoundException($"Replica directory '{replica}' does not exist. Parameter '{nameof(createReplicaDir)}' disabled auto-creation.");
            }
            rDirInfo.Create();
        }

        var worker = new SyncWorker()
        {
            SourcePath = source,
            TargetPath = replica,
            Period = cron.CronExpToTimeSpan(),
        };

        var process = new Process
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = typeof(SyncTool.Worker.Worker).Assembly.Location
                //Path.Combine(
                //    new FileInfo(Environment.ProcessPath).Directory,
                //    )
            }
        };
        // todo sync workers list
        process.Start();
    }
}

public static class CommandNames
{
    public const string sync = nameof(sync);
}

public static class Extensions
{
    /// <summary>
    ///     Translates cron expression to TimeSpan
    /// </summary>
    /// <param name="cron"></param>
    /// <remarks>
    ///     to simplify things, I'm skipping leap days, daylight savings etc
    /// </remarks>
    /// <returns></returns>
    public static TimeSpan CronExpToTimeSpan(this string cron)
    {
        CronExpression xd = CronExpression.Parse(cron);
        var occ1 = xd.GetNextOccurrence(DateTime.UtcNow);  // skip first one, in case delta is smaller
        var occ2 = xd.GetNextOccurrence(occ1.Value);
        var occ3 = xd.GetNextOccurrence(occ2.Value);
        return occ3.Value - occ2.Value;
    }
}