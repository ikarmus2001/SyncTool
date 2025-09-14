using ConsoleAppFramework;

namespace SyncTool.CLI;

internal class Program
{
    static void Main(string[] args)
    {
        var app = ConsoleApp.Create();
        app.Add<App>();
        app.Run(args);
    }
}
