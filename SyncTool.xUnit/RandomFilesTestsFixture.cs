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
        var files = sourceDirInfo.GetFiles("*", SearchOption.AllDirectories);

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
        var files = sourceDirInfo.GetFiles("*", SearchOption.AllDirectories);

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

public class RandomManyDirsFixture : ManySubdirsFixtureBase
{
    private ushort MaxDepth => field--;

    public RandomManyDirsFixture() : base()
    {
        MaxDepth = 10;
        var numberOfDirs = faker.Random.Int(3, 10);
        for (int i = 0; i < numberOfDirs; i++)
        {
            _ = CreateSampleDir();
        }
        return;

        DirectoryInfo CreateSampleDir()
        {
            var newDirName = faker.Random.Word();
            var newDir = new DirectoryInfo(
                Path.Combine(
                    SourcePath,
                    newDirName
                ));
            newDir.Create();
            logger?.LogInformation("Created sample directory: {dir name}", newDir.FullName);
            var numberOfFiles = faker.Random.UShort(0, 3);
            for (int i = 0; i < numberOfFiles; i++)
            {
                var newFileName = $"{faker.Random.Word()}.{faker.System.CommonFileExt()}";
                var paragraphsCount = faker.Random.Int(0, 2);
                CreateFile(Path.Combine(newDirName, newFileName), faker.Lorem.Paragraphs(paragraphsCount));
                logger?.LogInformation("Created sample file: {file name} with {paragraphCount} paragraph(s)", 
                    newFileName, paragraphsCount);
            }

            if (faker.Random.Bool(MaxDepth/10))
            {
                CreateSampleDir();
            }
            return newDir;
        }
    }

    internal override void AddChangesInFiles()
    {
        var sourceDirInfo = new DirectoryInfo(SourcePath);
        var files = sourceDirInfo.GetFiles("*", SearchOption.AllDirectories);

        var filesToMod = faker.PickRandom(files, faker.Random.Int(1, files.Length - 1));
        logger?.LogInformation("Modifying files: {file names}", filesToMod.Select(f => f.FullName + Environment.NewLine));
        foreach (var file in filesToMod)
        {
            OverwriteFile(file, faker.Lorem.Paragraphs(faker.Random.Int(1, 5)));
        }
    }

    internal override void DeleteFiles()
    {
        var sourceDirInfo = new DirectoryInfo(SourcePath);
        var files = sourceDirInfo.GetFiles("*", SearchOption.AllDirectories);

        var amountToPick = faker.Random.Int(1, files.Length - 1);
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