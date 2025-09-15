namespace SyncTool.xUnit;

public partial class HardcodedTests
{
    public class ManySubdirsFixture : ManySubdirsFixtureBase
    {
        private FileInfo toDelete;
        private FileInfo toUpdate;

        public ManySubdirsFixture() 
            : base()
        {
            var d1 = Directory.CreateDirectory(
                Path.Combine(SourcePath, faker.Random.Word())
            );
            toUpdate = CreateFile($"{d1.FullName}/TopLevelFile.txt", "# Header" + Environment.NewLine + "Some content");

            var d1_d2 = Directory.CreateDirectory(
                Path.Combine(d1.FullName, faker.Random.Word())
            );
            CreateFile($"{d1_d2.FullName}/SubdirFile1.txt", "# Headerasdfasdfasdf" + Environment.NewLine + "Some content");
            toDelete = CreateFile($"{d1_d2.FullName}/deleteMe.imfine", "# Headerasdfasdfasdf" + Environment.NewLine + "Some content");

            _ = Directory.CreateDirectory(
                Path.Combine(d1_d2.FullName, faker.Random.Word())
            );
            // empty

            var d1_d2_d3_2 = Directory.CreateDirectory(
                Path.Combine(d1_d2.FullName, faker.Random.Word())
            );
            CreateFile($"{d1_d2_d3_2.FullName}/Deepest.jaws", "# " + Environment.NewLine + "https://www.youtube.com/watch?v=BX3bN5YeiQs");
        }

        internal override void AddChangesInFiles()
        {
            OverwriteFile(toUpdate, "programatically changed content!");
        }

        internal override void DeleteFiles()
        {
            toDelete.Delete();
        }
    }
}
