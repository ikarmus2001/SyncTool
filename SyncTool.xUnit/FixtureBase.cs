using Bogus;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace SyncTool.xUnit;

public abstract class FixtureBase : IDisposable
{
    protected readonly Faker faker = new();
    public ILogger? logger;

    public string SourcePath { get; init; }
    public string TargetPath { get; init; }


    internal abstract void AddChangesInFiles();
    internal abstract void DeleteFiles();


    protected FixtureBase()
    {
        SourcePath = Path.Combine(
            Path.GetTempPath(),
            faker.Random.Word()
        );
        if (Directory.Exists(SourcePath)) { Directory.Delete(SourcePath, recursive: true); }
        Directory.CreateDirectory(SourcePath);

        TargetPath = Path.Combine(
            Path.GetTempPath(),
            faker.Random.Word()
        );
        if (Directory.Exists(TargetPath)) { Directory.Delete(TargetPath, recursive: true); }
        Directory.CreateDirectory(TargetPath);
    }

    internal void InitLog(ITestOutputHelper logOutput)
    {
        logger = new Logging(logOutput).Logger;
    }

    protected FileInfo CreateFile(string name, string content, bool atSource = true)
    {
        var file = new FileInfo(
            Path.Combine(
                atSource ? SourcePath : TargetPath,
                name
            ));
        using var streamWriter = file.CreateText();
        streamWriter.Write(content);
        return file;
    }

    protected static void OverwriteFile(FileInfo file, string content)
    {
        using var sw = file.CreateText();
        sw.Write(content);
    }

    public void Dispose()
    {
        Directory.Delete(SourcePath, recursive: true);
        Directory.Delete(TargetPath, recursive: true);
    }
}
