using Microsoft.Extensions.Logging;
using SyncTool.Lib;
using Xunit.Abstractions;

namespace SyncTool.xUnit;

public class PlainFilesTests : IClassFixture<PlainFilesTestsFixture>
{
    private PlainFilesTestsFixture _fixture;

    public PlainFilesTests(PlainFilesTestsFixture fixture, ITestOutputHelper logOutput)
    {
        _fixture = fixture;
        _fixture.InitLog(logOutput);
    }

    [Fact]
    public void FilesAreCopiedToEmptyDirectory()
    {
        SyncWorker syncWorker = new()
        {
            SourcePath = _fixture.SourcePath,
            TargetPath = _fixture.TargetPath,
            Period = TimeSpan.Zero
        };

        syncWorker.SyncFiles();

        AssertDirsContainsEquivalentFiles(_fixture.SourcePath, _fixture.TargetPath);
    }

    [Fact]
    public void FilesAreUpdatedInTargetDirectory()
    {
        _fixture.AddChangesInFiles();

        SyncWorker syncWorker = new()
        {
            SourcePath = _fixture.SourcePath,
            TargetPath = _fixture.TargetPath,
            Period = TimeSpan.Zero
        };
        syncWorker.SyncFiles();

        AssertDirsContainsEquivalentFiles(_fixture.SourcePath, _fixture.TargetPath);
    }

    [Fact]
    public void FilesAreDeletedInTargetDirectory()
    {
        _fixture.DeleteRandomFiles();
        SyncWorker syncWorker = new()
        {
            SourcePath = _fixture.SourcePath,
            TargetPath = _fixture.TargetPath,
            Period = TimeSpan.Zero
        };
        syncWorker.SyncFiles();

        AssertDirsContainsEquivalentFiles(_fixture.SourcePath, _fixture.TargetPath);
    }

    private void AssertDirsContainsEquivalentFiles(string sourceFilePath, string targetFilePath)
    {
        var movedFiles = Directory.GetFiles(_fixture.TargetPath, "*", SearchOption.AllDirectories)
            .Select(f => Path.GetFileName(f));
        _fixture.logger?.LogInformation("Moved files: {movedFiles}", movedFiles.Select(f => f + Environment.NewLine));

        Assert.NotEmpty(movedFiles);

        var sourceFiles = Directory.GetFiles(_fixture.SourcePath, "*", SearchOption.AllDirectories)
            .Select(f => Path.GetFileName(f));
        _fixture.logger?.LogInformation("Source files: {sourceFiles}", sourceFiles.Select(f => f + Environment.NewLine));

        Assert.Equivalent(sourceFiles, movedFiles);
    }

    // assure input directory exist
    // assure output directory exist (default create, flag for using only )
    // assure identical (to metadata level)

}