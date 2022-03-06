using System.Runtime.InteropServices;

namespace SerialOverWebsocketClient.PseudoTerminal;

class PosixPseudoTerminal : IPseudoTerminal
{
    public event Action<byte[], int>? OnRead;

    private const string LIBRARY_NAME = "pts.so";
    private readonly int fileHandle;
    private readonly Task readTask;

    public string Filename { get; }

    [DllImport(LIBRARY_NAME)]
    private static extern int CreatePTY(out int master, out int slave);

    [DllImport(LIBRARY_NAME)]
    private static extern IntPtr GetSlavePTY(int handle);

    [DllImport(LIBRARY_NAME)]
    private static extern int ReadPTY(int fd, byte[] buffer, int length);

    [DllImport(LIBRARY_NAME)]
    private static extern int WritePTY(int fd, byte[] buffer, int length);

    [DllImport(LIBRARY_NAME)]
    private static extern int ClosePTY(int fd);

    public PosixPseudoTerminal()
    {
        if (CreatePTY(out var masterHandle, out _) != 0)
            throw new Exception("CreatePTY failed");

        fileHandle = masterHandle;
        Filename = Marshal.PtrToStringAnsi(GetSlavePTY(fileHandle));
        readTask = Task.Run(ReadLoop);
    }

    public void Dispose()
    {
        ClosePTY(fileHandle);
        readTask.Wait();
    }

    public void Write(byte[] buffer, int length)
    {
        WritePTY(fileHandle, buffer, length);
    }

    private async Task ReadLoop()
    {
        var buffer = new byte[4096];
        while (true)
        {
            var read = ReadPTY(fileHandle, buffer, buffer.Length);
            if (read == -1)
            {
                await Task.Delay(100);
            }
            else
            {
                OnRead?.Invoke(buffer, read);
            }
        }
    }
}