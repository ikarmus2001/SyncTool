using SyncTool.Lib;

namespace SyncTool.xUnit;

public class MovingFilesTests
{
    /// <summary>
    ///     Repo path, as it seems to be a reasonable case
    /// </summary>
    private readonly string sourcePath = Path.GetFullPath(Environment.CurrentDirectory).TillLastOccurence($"{nameof(SyncTool)}.{nameof(xUnit)}");
    private readonly string targetPath = Path.Combine(Environment.CurrentDirectory, "Tests", "Target");

    [Fact]
    public void FilesShouldBeMovedToEmptyDirectory()
    {
        var xd = Directory.Exists(targetPath);
        Assert.False(xd);

        SyncWorker syncWorker = new()
        {
            SourcePath = sourcePath,
            TargetPath = targetPath,
            Period = TimeSpan.Zero
        };
        try
        {

            syncWorker.MoveFiles();

            var movedFiles = Directory.GetFiles(targetPath, "*", SearchOption.AllDirectories);
            Assert.NotEmpty(movedFiles);

            var sourceFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
            Assert.Equivalent(sourceFiles, movedFiles);
        }
        finally
        {
            Directory.Delete(targetPath, recursive: true);
        }
    }

    // assure input directory exist
    // assure output directory exist (default create, flag for using only )
    // assure identical (to metadata level)

}