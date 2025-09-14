namespace SyncTool.Lib;

public class SyncWorker
{
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }
    public TimeSpan Period { get; set; }

    public void SyncFiles()
    {
        DirectoryInfo sourceDirInfo = new(SourcePath);
        DirectoryInfo targetDirInfo = !Directory.Exists(TargetPath)  // && TODO flag for error
            ? Directory.CreateDirectory(TargetPath)
            : new DirectoryInfo(TargetPath);

        IEnumerable<FileSystemInfo> targetFSInfos = targetDirInfo.EnumerateFileSystemInfos();
        if (targetFSInfos.Count() == 0)  // optimization for empty target dir
        {
            sourceDirInfo.CopyTo(targetDirInfo);
            return;
        }

        var targetFsHandling = targetFSInfos
            .Select(tfi => new FileSysInfoHandling() { handled = false, fsInfo = tfi });

        var sourceFsHandling = sourceDirInfo.EnumerateFileSystemInfos()
            .Select(sfi => new FileSysInfoHandling() { handled = false, fsInfo = sfi });

        IEnumerable<(FileSysInfoHandling source, FileSysInfoHandling?)> pairedFSInfos = 
            sourceFsHandling.Zip(targetFsHandling,
                (source, target) => source.fsInfo.Name == target.fsInfo.Name
                                    ? (source, target) 
                                    : (source, null)
        );


        foreach (var (source, target) in pairedFSInfos)
        {
            bool forceCopy = target is null;

            if (source.fsInfo is DirectoryInfo sDirInfo)
            {
                //if (forceCopy || Validation(sDirInfo, (DirectoryInfo)target.fsInfo))
                //{
                //    sDirInfo.CopyTo(targetDirInfo);
                //}
            }
            else if (source.fsInfo is FileInfo sFileInfo)
            {
                if (forceCopy || !IsUpToDate(sFileInfo, (FileInfo)target.fsInfo))
                {
                    sFileInfo.CopyTo(Path.Combine(targetDirInfo.FullName, sFileInfo.Name), overwrite: true);
                }
            }
            source.handled = true;
            target?.handled = true;
        }

        DeleteFiles();
        return;


        static bool IsUpToDate(FileInfo f1, FileInfo f2)
        {
            return f1.Length == f2.Length 
                && f1.LastWriteTime == f2.LastWriteTime;
        }

        void DeleteFiles()
        {
            var filesToDelete = targetFsHandling
                .Where(t => !sourceFsHandling.Any(s => s.fsInfo.Name == t.fsInfo.Name));

            foreach (var fileToDelete in filesToDelete)
            {
                switch (fileToDelete.fsInfo)
                {
                    case DirectoryInfo dirInfo:
                        dirInfo.Delete(recursive: true);
                        break;
                    case FileInfo fileInfo:
                        fileInfo.Delete();
                        break;
                }
            }
        }
    }

    private class FileSysInfoHandling
    {
        public bool handled;
        public FileSystemInfo fsInfo;
    }
}
