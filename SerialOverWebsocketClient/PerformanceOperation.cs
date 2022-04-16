using System.Diagnostics;

namespace SerialOverWebsocketClient;

class PerformanceOperation : IDisposable
{
    private readonly string format;
    Stopwatch stopWatch = new();

    public PerformanceOperation(string format)
    {
        this.format = format;
        stopWatch.Start();
    }

    public void Dispose()
    {
        stopWatch.Stop();
        var ts = stopWatch.Elapsed;
        Console.WriteLine(format, ts.TotalMilliseconds);
    }
}