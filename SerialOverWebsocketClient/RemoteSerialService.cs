using Microsoft.Extensions.Options;
using WebSocketSharp;

namespace SerialOverWebsocketClient;

public class RemoteSerialService
{
    public event Action<string> OnSerialData;

    private readonly AuthorizationService authorizationService;
    private readonly ConnectionOptions options;
    private readonly RemoteSession session;

    public RemoteSerialService(IOptions<ConnectionOptions> _options, AuthorizationService authorizationService)
    {
        this.authorizationService = authorizationService;
        options = _options.Value;
        var socket = new WebSocket(options.Url);
        session = new RemoteSession(socket, authorizationService);
        session.OnClose += SocketOnClose;
        session.OnOpen += SocketOnOpen;
        session.OnAuthorized += SocketOnAuthorized;
        session.OnData += SocketOnData;
        session.OnStatus += msg => Console.WriteLine($"SESSION: {msg}");
    }

    public void Start()
    {
        session.Connect();
    }

    public void SetSwitch(bool value)
    {
        session.WriteSwitch(value);
    }

    public void WriteSerialData(string value)
    {
        Console.WriteLine($">%{value}%\n");
        session.WriteData(value);
    }

    private void SocketOnOpen()
    {
        Console.WriteLine("Unauthorized connection achieved");
    }

    private void SocketOnAuthorized()
    {
        Console.WriteLine("Authorized connection achieved");
    }

    private void SocketOnData(string value)
    {
        Console.WriteLine($"<%{value}%\n");
        OnSerialData?.Invoke(value);
    }

    private void SocketOnClose()
    {
        Task.Delay(options.ErrorTimeout)
            .ContinueWith((_) => session.Connect());
    }
}