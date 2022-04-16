using System.Text;
using System.Text.Json;
using SerialOverWebsocketClient.Messages.Requests;
using SerialOverWebsocketClient.Messages.Responses;
using WebSocketSharp;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace SerialOverWebsocketClient;

class RemoteSession
{
    public event Action OnOpen = delegate { };
    public event Action OnAuthorized = delegate { };
    public event Action OnClose = delegate { };
    public event Action<string> OnData = delegate { };
    public event Action<string> OnStatus = delegate { };

    private readonly WebSocket socket;
    private readonly AuthorizationService authorizationService;
    private bool isAuthorizationAvailable = false;
    private bool isWorking = false;

    public RemoteSession(WebSocket socket, AuthorizationService authorizationService)
    {
        this.socket = socket;
        this.authorizationService = authorizationService;
        socket.OnClose += SocketOnClose;
        socket.OnOpen += SocketOnOpen;
        socket.OnError += SocketOnError;
        socket.OnMessage += SocketOnMessage;
    }

    public void Connect()
    {
        socket.Connect();
    }

    public void WriteSwitch(bool value)
    {
        SendMessage(new SwitchMessage {Data = value});
    }

    public void WriteData(string value)
    {
        //using var timer = new PerformanceOperation("WriteData {0}ms"); 
        var bytes = Encoding.UTF8.GetBytes(value);
        var encoded = Convert.ToBase64String(bytes);
        SendMessage(new WriteMessage {Data = encoded});
    }

    private void SocketOnMessage(object? sender, MessageEventArgs args)
    {
        if (!TryDeserializeMessage(args.Data))
            socket.Close();
    }

    private void SocketOnOpen(object? sender, EventArgs e)
    {
        OnOpen?.Invoke();
        isAuthorizationAvailable = true;
        SendMessage(new RequestAuthMessage());
    }

    private void SocketOnError(object? sender, ErrorEventArgs e)
    {
        OnClose?.Invoke();
    }

    private void SocketOnClose(object? sender, CloseEventArgs e)
    {
        OnClose?.Invoke();
    }

    private void SendMessage<T>(T message) where T : SessionMessage
    {
        //using var timer = new PerformanceOperation("SendMessage {0}ms"); 
        var data = JsonSerializer.Serialize(message);
        socket.Send(data);
    }

    private bool TryDeserializeMessage(string data)
    {
        try
        {
            return DeserializeMessage(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return false;
    }

    private bool DeserializeMessage(string data)
    {
        //using var timer = new PerformanceOperation("DeserializeMessage {0}ms"); 
        var basicMessage = JsonSerializer.Deserialize<BasicSessionMessage>(data);
        if (basicMessage == null)
            return false;

        if (basicMessage.Type == AuthMessage.DefaultType)
        {
            var message = JsonSerializer.Deserialize<AuthMessage>(data);
            OnAuthMessage(message);
            return true;
        }

        if (basicMessage.Type == AuthorizedMessage.DefaultType)
        {
            var message = JsonSerializer.Deserialize<AuthorizedMessage>(data);
            OnAuthorizedMessage(message);
            return true;
        }

        if (basicMessage.Type == StatusMessage.DefaultType)
        {
            var message = JsonSerializer.Deserialize<StatusMessage>(data);
            OnStatusMessage(message);
            return true;
        }

        if (basicMessage.Type == ReadMessage.DefaultType)
        {
            var message = JsonSerializer.Deserialize<ReadMessage>(data);
            OnReadMessage(message);
            return true;
        }

        return false;
    }

    private void OnAuthMessage(AuthMessage message)
    {
        if (!isAuthorizationAvailable)
            return;

        isAuthorizationAvailable = false;

        var token = message.GetBytes();
        if (token == null)
            return;

        var hash = authorizationService.CreateVerifyToken(token);
        if (hash == null)
            return;

        SendMessage(new ResponseAuthMessage(hash));
    }

    private void OnAuthorizedMessage(AuthorizedMessage message)
    {
        OnAuthorized?.Invoke();
    }

    private void OnReadMessage(ReadMessage message)
    {
        //using var timer = new PerformanceOperation("OnReadMessage {0}ms");
        var bytes = Convert.FromBase64String(message.Data);
        var data = Encoding.UTF8.GetString(bytes);
        OnData?.Invoke(data);
    }

    private void OnStatusMessage(StatusMessage message)
    {
        if (message.Action == "switch")
        {
            isWorking = message.Status;
        }

        if (message.Action == "write" && message.Status)
            return;

        OnStatus?.Invoke($"{message.Action} => {message.Status}");
    }
}