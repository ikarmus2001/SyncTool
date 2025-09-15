using Cronos;
using SyncTool.Lib;
using System.Diagnostics;
using System.IO.Pipes;
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
    /// <remarks>
    ///     Spawns worker in background process (started if not running already)
    /// </remarks>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    [Command(CommandNames.sync)]
    public void Sync(string source, string replica, string cron = "*/2 * * * *", bool createReplicaDir = true)
    {
        logger.Information($"{CommandNames.sync} invoked");
        var sDirInfo = new DirectoryInfo(source);  // validate source dir
        var rDirInfo = new DirectoryInfo(replica);
        if (!sDirInfo.Exists)
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
        
        _ = GetBackgroundProcess();

        ScheduleSync();
        return;


        Process GetBackgroundProcess()
        {
            var workerProcesses = Process.GetProcessesByName(typeof(Worker.Worker).Assembly.GetName().Name)
                .ToList();

            Process workerProcess = workerProcesses.Count == 0
                ? SpawnBackgroundWorker()
                : workerProcesses.First();

            if (workerProcesses.Count > 1)
            {
                logger.Warning("Multiple sync worker processes detected. This might lead to unexpected behavior.");
            }
            logger.Information($"Communicating with {workerProcess.Id}");
            return workerProcess;


            static Process SpawnBackgroundWorker()
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = "dotnet",
                        Arguments = typeof(Worker.Worker).Assembly.Location
                    }
                };
                process.Start();
                return process;
            }
        }

        void ScheduleSync()
        {
            // Prepare config
            var worker = new SyncWorker()
            {
                SourcePath = source,
                TargetPath = replica,
                Period = cron.CronExpToTimeSpan(),
            };
            var workerConfig = System.Text.Json.JsonSerializer.Serialize(worker, SyncTool.Lib.Communication.Constants.JsonSerializerOptions);

            NamedPipeClientStream namedPipeClientStream = new(SyncTool.Lib.Communication.Constants.PipeName);
            namedPipeClientStream.Connect(TimeSpan.FromSeconds(5));

            using (StreamWriter sw = new(namedPipeClientStream, leaveOpen: true))
            {
                sw.WriteLine(workerConfig);
                sw.Flush();
            }

            using StreamReader sr = new(namedPipeClientStream);
            var response = sr.ReadLine();

            var createWorkerResponse = System.Text.Json.JsonSerializer.Deserialize<Lib.Communication.CreateWorkerResponse>(response);
            logger.Information($"Got worker id: '{createWorkerResponse?.createdWorkerId}'");
        }
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