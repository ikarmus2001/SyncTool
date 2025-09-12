namespace SyncTool.xUnit;

internal static class Extensions
{
    /// <summary>
    ///     Gets the path substring till the most nested occurence of a <paramref name="searchDir"/>
    /// </summary>
    /// <param name="path"></param>
    /// <param name="searchDir"></param>
    /// <returns></returns>
    internal static string TillLastOccurence(this string path, in string searchDir)
    {
        var lIdx = path.LastIndexOf(searchDir);
        return path.Substring(0, lIdx + searchDir.Length + 1);
    }
}
