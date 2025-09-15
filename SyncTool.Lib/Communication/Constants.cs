namespace SyncTool.Lib.Communication;

public class Constants
{
    public const string PipeName = "SyncToolPipe";

    public static readonly System.Text.Json.JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = false
    };
}
