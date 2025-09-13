using Bogus;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace SyncTool.xUnit
{
    public abstract class PlainFilesTestsFixtureBase
    {
        protected readonly Faker faker = new();
        public ILogger? logger;

        public string SourcePath { get; init; }
        public string TargetPath { get; init; }

        public PlainFilesTestsFixtureBase()
        {
            SourcePath = Path.Combine(
                Path.GetTempPath(),
                faker.Random.Word()
            );
            if (Directory.Exists(SourcePath)) { Directory.Delete(SourcePath, recursive: true); }
            Directory.CreateDirectory(SourcePath);

            TargetPath = Path.Combine(
                Path.GetTempPath(),
                faker.Random.Word()
            );
            if (Directory.Exists(TargetPath)) { Directory.Delete(TargetPath, recursive: true); }
            Directory.CreateDirectory(TargetPath);
        }

        internal void InitLog(ITestOutputHelper logOutput)
        {
            logger = new Logging(logOutput).Logger;
        }


        internal abstract void AddChangesInFiles();
        internal abstract void DeleteFiles();
        
        
        protected void CreateFile(string name, string content, bool atSource = true)
        {
            using var streamWriter = File.CreateText(
                Path.Combine(
                    atSource ? SourcePath : TargetPath,
                    name
                )
            );
            streamWriter.Write(content);
        }

        protected static void OverwriteFile(FileInfo file, string content)
        {
            using var sw = file.CreateText();
            sw.Write(content);
        }


        public void Dispose()
        {
            Directory.Delete(SourcePath, recursive: true);
            Directory.Delete(TargetPath, recursive: true);
        }
    }
}