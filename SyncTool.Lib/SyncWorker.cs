namespace SyncTool.Lib;

public class SyncWorker
{
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }
    public TimeSpan Period { get; set; }

    public void SyncFiles()
    {
        DirectoryInfo source = new(SourcePath);
        DirectoryInfo target = !Directory.Exists(TargetPath)  // && TODO flag for error
            ? Directory.CreateDirectory(TargetPath) 
            : new DirectoryInfo(TargetPath);

        IEnumerable<FileSystemInfo> targetFSInfos = target.EnumerateFileSystemInfos();
        
        if (targetFSInfos.Count() == 0)  // optimization for empty target dir
        {
            source.CopyTo(target);
            return;
        }

        IEnumerable<FileSystemInfo> sourceFSInfos = source.EnumerateFileSystemInfos();
        var pairedFSInfos = sourceFSInfos.Zip(targetFSInfos, 
                            resultSelector: (source, target) => source.Name == target.Name 
                                                                ? (source, target) 
                                                                : (source, null)
        );


        foreach ((FileSystemInfo source, FileSystemInfo? target) fsInfoPair in pairedFSInfos)
        {
            if (target is null)
            {
                source.CopyTo(target);
                continue;
            }

            if (fsInfoPair.target is FileInfo targetFile)
            {
                if (fsInfoPair.source.LastWriteTimeUtc != targetFile.LastWriteTimeUtc)
                {
                    //sourceFile.CopyTo(targetFile.FullName, overwrite: true);
                }
            }
            else if (fsInfoPair.target is DirectoryInfo targetDir)
            {
                
            }
        }
    }
}
