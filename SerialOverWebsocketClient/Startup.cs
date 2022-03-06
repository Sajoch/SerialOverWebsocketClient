using SerialOverWebsocketClient.PseudoTerminal;

namespace SerialOverWebsocketClient;

class Startup
{
    private readonly RemoteSerialService remoteSerialService;
    private readonly PseudoTerminalService terminal;

    public Startup(RemoteSerialService remoteSerialService, PseudoTerminalService terminal)
    {
        this.remoteSerialService = remoteSerialService;
        this.terminal = terminal;
    }

    public async Task Run()
    {
        remoteSerialService.Start();
        terminal.OnSwitchChanged += remoteSerialService.SetSwitch;
        terminal.OnSerialData += remoteSerialService.WriteSerialData;
        remoteSerialService.OnSerialData += terminal.WriteSerialData;
        await Task.Delay(Timeout.Infinite);
    }
}