namespace SerialOverWebsocketClient.PseudoTerminal;

public class PseudoTerminalService : IDisposable
{
    public event Action<bool> OnSwitchChanged;
    public event Action<string> OnSerialData;
    private readonly IPseudoTerminal serialPort;
    private readonly IPseudoTerminal switchPort;

    public PseudoTerminalService(IPseudoTerminal serialPort, IPseudoTerminal switchPort)
    {
        this.serialPort = serialPort;
        this.switchPort = switchPort;

        File.WriteAllText("env", @$"SERIAL={serialPort.Filename}\nSWTICH={switchPort.Filename}\n");

        serialPort.OnRead += OnSerialPortRead;
        switchPort.OnRead += OnSwitchPortRead;
    }

    public void Dispose()
    {
        serialPort?.Dispose();
        switchPort?.Dispose();
    }

    public void WriteSerialData(string message)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(message);
        serialPort?.Write(bytes, bytes.Length);
    }

    private void OnSerialPortRead(byte[] buffer, int length)
    {
        var content = System.Text.Encoding.UTF8.GetString(buffer, 0, length);
        OnSerialData?.Invoke(content);
    }

    private void OnSwitchPortRead(byte[] buffer, int length)
    {
        var content = System.Text.Encoding.UTF8.GetString(buffer, 0, length);
        var value = content.Trim();
        if (value == "1")
        {
            OnSwitchChanged?.Invoke(true);
        }
        else if (value == "0")
        {
            OnSwitchChanged?.Invoke(false);
        }
    }
}