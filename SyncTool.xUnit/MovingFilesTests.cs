using SyncTool.Lib;

namespace SyncTool.xUnit;

public class MovingFilesTests
{
    private readonly string sourcePath = Path.Combine(Environment.CurrentDirectory, "Tests", "Source");
    private readonly string targetPath = Path.Combine(Environment.CurrentDirectory, "Tests", "Target");

    [Fact]
    public void FilesShouldBeMoved()
    {
        SyncWorker syncWorker = new()
        {
            SourcePath = sourcePath,
            TargetPath = targetPath,
            Period = TimeSpan.Zero
        };

        syncWorker.MoveFiles();

        var movedFiles = Directory.GetFiles(targetPath, "*", SearchOption.AllDirectories);
        Assert.NotEmpty(movedFiles);

        var sourceFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
        Assert.Equivalent(sourceFiles, movedFiles);
    }

    // assure input directory exist
    // assure output directory exist (default create, flag for using only )
    // assure identical (to metadata level)

}