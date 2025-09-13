namespace SyncTool.xUnit;

public partial class HardcodedTests
{
    public class PlainFilesFixture : PlainFilesTestsFixtureBase
    {
        public PlainFilesFixture()
            : base()
        {
            CreateFile("SomeFileName.txt", "Lorem");
            CreateFile("AnotherFileName.md", "# Header" + Environment.NewLine + "Some content");
            CreateFile("Image.png", "Not really an image but a text file with image extension");
        }

        internal override void AddChangesInFiles()
        {
            var sourceDirInfo = new DirectoryInfo(SourcePath);
            var files = sourceDirInfo.GetFiles().First(f => f.Name == "Image.png");
            OverwriteFile(files, "Overwritten content");
        }

        internal override void DeleteFiles()
        {
            var sourceDirInfo = new DirectoryInfo(SourcePath);
            var files = sourceDirInfo.GetFiles();

            var fileToDelete = files.First(f => f.Name == "AnotherFileName.md");
            fileToDelete.Delete();
        }
    }
}
