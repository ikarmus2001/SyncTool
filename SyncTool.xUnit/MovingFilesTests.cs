using SyncTool.Lib;

namespace SyncTool.xUnit;

public class MovingFilesTests(MovingFilesTestsFixture fixture) : IClassFixture<MovingFilesTestsFixture>
{
    [Fact]
    public void FilesAreCopiedToEmptyDirectory()
    {
        SyncWorker syncWorker = new()
        {
            SourcePath = fixture.sourcePath,
            TargetPath = fixture.targetPath,
            Period = TimeSpan.Zero
        };

        syncWorker.MoveFiles();

        var movedFiles = Directory.GetFiles(fixture.targetPath, "*", SearchOption.AllDirectories)
            .Select(f => Path.GetFileName(f));
        Assert.NotEmpty(movedFiles);

        var sourceFiles = Directory.GetFiles(fixture.sourcePath, "*", SearchOption.AllDirectories)
            .Select(f => Path.GetFileName(f));
        Assert.Equivalent(sourceFiles, movedFiles);
    }

    // assure input directory exist
    // assure output directory exist (default create, flag for using only )
    // assure identical (to metadata level)

}