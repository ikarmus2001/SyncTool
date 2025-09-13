using Bogus;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace SyncTool.xUnit;

public class PlainFilesTestsFixture : IDisposable
{
    private readonly Faker faker = new();
    public ILogger? logger;

    public string SourcePath { get; init; }
    public string TargetPath { get; init; }

    public PlainFilesTestsFixture()
    {
        SourcePath = Path.Combine(
            Path.GetTempPath(),
            faker.Random.Word()
        );
        if (Directory.Exists(SourcePath)) { Directory.Delete(SourcePath); }
        Directory.CreateDirectory(SourcePath);

        TargetPath = Path.Combine(
            Path.GetTempPath(),
            faker.Random.Word()
        );
        if (Directory.Exists(TargetPath)) { Directory.Delete(TargetPath); }
        Directory.CreateDirectory(TargetPath);

        var numberOfFiles = faker.Random.Int(5, 20);
        for (int i = 0; i < numberOfFiles; i++)
        {
            CreateSampleFile();
        }
        return;


        void CreateSampleFile()
        {
            using var streamWriter = File.CreateText(
                Path.Combine(
                    SourcePath, 
                    $"{faker.Random.Word()}.{faker.System.CommonFileExt()}"
                )
            );
            streamWriter.Write(faker.Lorem.Paragraphs(faker.Random.Int(1, 5)));
        }
    }

    internal void InitLog(ITestOutputHelper logOutput)
    {
        logger = new Logging(logOutput).Logger;
    }

    internal void AddChangesInFiles()
    {
        var sourceDirInfo = new DirectoryInfo(SourcePath);
        var files = sourceDirInfo.GetFiles();

        var filesToMod = faker.PickRandom(files, faker.Random.Int(1, files.Length));
        logger?.LogInformation("Modifying files: {file names}", filesToMod.Select(f => f.FullName + Environment.NewLine));
        foreach (var file in filesToMod)
        {
            using var sw = file.CreateText();
            sw.Write(faker.Lorem.Paragraphs(faker.Random.Int(1, 5)));
        }
    }

    public void Dispose()
    {
        Directory.Delete(SourcePath, recursive: true);
        Directory.Delete(TargetPath, recursive: true);
    }

    internal void DeleteRandomFiles()
    {
        var sourceDirInfo = new DirectoryInfo(SourcePath);
        var files = sourceDirInfo.GetFiles();

        var filesToDelete = faker.PickRandom(files, faker.Random.Int(1, files.Length));
        logger?.LogInformation("Deleting files: {file names}", filesToDelete.Select(f => f.FullName + Environment.NewLine));
        foreach (var file in filesToDelete)
        {
            file.Delete();
        }
    }
}
