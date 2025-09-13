using Bogus;

namespace SyncTool.xUnit;

public class MovingFilesTestsFixture : IDisposable
{
    private Faker faker = new();

    public string sourcePath { get; init; }
    public string targetPath { get; init; }

    public MovingFilesTestsFixture()
    {
        sourcePath = Path.Combine(
            Path.GetTempPath(),
            faker.Random.Word()
        );
        if (Directory.Exists(sourcePath)) { Directory.Delete(sourcePath); }
        Directory.CreateDirectory(sourcePath);

        targetPath = Path.Combine(
            Path.GetTempPath(),
            faker.Random.Word()
        );
        if (Directory.Exists(targetPath)) { Directory.Delete(targetPath); }
        Directory.CreateDirectory(targetPath);

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
                    sourcePath, 
                    $"{faker.Random.Word()}.{faker.System.CommonFileExt()}"
                )
            );
            streamWriter.Write(faker.Lorem.Paragraphs(faker.Random.Int(1, 5)));
        }
    }


    public void Dispose()
    {
        Directory.Delete(sourcePath, recursive: true);
        Directory.Delete(targetPath, recursive: true);
    }
}
