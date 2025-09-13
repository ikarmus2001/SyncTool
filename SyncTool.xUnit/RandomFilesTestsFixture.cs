using Microsoft.Extensions.Logging;

namespace SyncTool.xUnit;

public class RandomFilesTestsFixture : PlainFilesTestsFixtureBase
{
    public RandomFilesTestsFixture() : base()
    {
        var numberOfFiles = faker.Random.Int(5, 20);
        for (int i = 0; i < numberOfFiles; i++)
        {
            CreateSampleFile();
        }
        return;


        void CreateSampleFile()
        {
            var newFileName = $"{faker.Random.Word()}.{faker.System.CommonFileExt()}";
            var paragraphsCount = faker.Random.Int(1, 5);
             
            CreateFile(newFileName, faker.Lorem.Paragraphs(paragraphsCount));
            logger?.LogInformation("Created sample file: {file name} with {paragraphCount} paragraph(s)", 
                newFileName, paragraphsCount);
        }
    }

    internal override void AddChangesInFiles()
    {
        var sourceDirInfo = new DirectoryInfo(SourcePath);
        var files = sourceDirInfo.GetFiles();

        var filesToMod = faker.PickRandom(files, faker.Random.Int(1, files.Length-1));
        logger?.LogInformation("Modifying files: {file names}", filesToMod.Select(f => f.FullName + Environment.NewLine));
        foreach (var file in filesToMod)
        {
            OverwriteFile(file, faker.Lorem.Paragraphs(faker.Random.Int(1, 5)));
        }
    }

    internal override void DeleteFiles()
    {
        var sourceDirInfo = new DirectoryInfo(SourcePath);
        var files = sourceDirInfo.GetFiles();

        var amountToPick = faker.Random.Int(1, files.Length-1);
        var filesToDelete = faker.PickRandom(files, amountToPick);
        logger?.LogInformation("Deleting {count}/{all} files: {file names}",  
            amountToPick, files.Length,
            filesToDelete.Select(f => f.FullName + Environment.NewLine));
        foreach (var file in filesToDelete)
        {
            file.Delete();
        }
    }
}
