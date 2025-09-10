namespace SyncTool.Lib;

public class SyncWorker
{
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }
    public TimeSpan Period { get; set; }

    public void MoveFiles()
    {
        throw new NotImplementedException();
    }
}
