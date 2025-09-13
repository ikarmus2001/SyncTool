using Microsoft.Extensions.Logging;
using SyncTool.Lib;
using Xunit.Abstractions;

namespace SyncTool.xUnit;

public abstract class PlainFilesTests<TFixture> : IClassFixture<TFixture>
    where TFixture : PlainFilesTestsFixtureBase
{
    private TFixture _fixture;

    public PlainFilesTests(TFixture fixture, ITestOutputHelper logOutput)
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
        _fixture.DeleteFiles();

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
        _fixture.logger?.LogInformation("Target files ({count}): {movedFiles}", 
            movedFiles.Count(), movedFiles.Select(f => f + Environment.NewLine));

        var sourceFiles = Directory.GetFiles(_fixture.SourcePath, "*", SearchOption.AllDirectories)
            .Select(f => Path.GetFileName(f));
        _fixture.logger?.LogInformation("Source files ({count}): {sourceFiles}", 
            sourceFiles.Count(), sourceFiles.Select(f => f + Environment.NewLine));

        Assert.Equivalent(sourceFiles, movedFiles);
    }

    // assure input directory exist
    // assure output directory exist (default create, flag for using only )
    // assure identical (to metadata level)

}

public class RandomPlainFilesTests : PlainFilesTests<RandomFilesTestsFixture>, IClassFixture<RandomFilesTestsFixture>
{
    public RandomPlainFilesTests(RandomFilesTestsFixture fixture, ITestOutputHelper logOutput) 
        : base(fixture, logOutput) { }
}

public class HardcodedPlainFilesTests : PlainFilesTests<HardcodedTests.PlainFilesFixture>, IClassFixture<HardcodedTests.PlainFilesFixture>
{
    public HardcodedPlainFilesTests(HardcodedTests.PlainFilesFixture fixture, ITestOutputHelper logOutput) 
        : base(fixture, logOutput) { }
}