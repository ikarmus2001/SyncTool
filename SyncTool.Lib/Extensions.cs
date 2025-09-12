using System.Diagnostics;

namespace SyncTool.Lib;

internal static class Extensions
{
    /// <summary>
    ///     Recursively copies directories and files
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns>Accumulated, recursive amount of files copied</returns>
    internal static uint CopyTo(this DirectoryInfo source, DirectoryInfo target)
    {
        uint result = 0u;
        foreach (FileInfo sfile in source.GetFiles())
        {
            result += CopyFileInfo(target, sfile);
        }
        foreach (DirectoryInfo dir in source.GetDirectories())
        {
            DirectoryInfo targetSubDir = target.CreateSubdirectory(dir.Name);
            result += dir.CopyTo(targetSubDir);
        }
        return result;
    }

    //internal static uint CopyTo(this FileInfo source, string targetPath)
    //{
        
    //}

    private static uint CopyFileInfo(DirectoryInfo target, FileInfo source)
    {
        try
        {
            var fullName = Path.Combine(target.FullName, source.Name);
            source.CopyTo(fullName, overwrite: true);
            return 1;
        }
        catch (UnauthorizedAccessException uae)
        {
            Debug.WriteLine(uae.Message);
        }
        return 0;
    }
}
