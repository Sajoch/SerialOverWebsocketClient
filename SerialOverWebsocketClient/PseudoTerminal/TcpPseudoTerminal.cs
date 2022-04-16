using System.Net;
using System.Net.Sockets;

namespace SerialOverWebsocketClient.PseudoTerminal;

public class TcpPseudoTerminal : IPseudoTerminal
{
    public event Action<byte[], int> OnRead;
    private readonly IPAddress ipAddress = IPAddress.Any;
    private TcpListener listener;
    private readonly List<NetworkStream> clients = new();

    public string Filename { get; private set; }

    public void Start(int index)
    {
        if (listener != null)
            throw new Exception($"{nameof(TcpPseudoTerminal)} already started!");

        var port = 8000 + index;
        listener = new TcpListener(ipAddress, port);
        Filename = listener.LocalEndpoint.ToString();
        listener.Start();

        Task.Run(Listen);
    }

    public void Write(byte[] buffer, int length)
    {
        foreach (var client in clients)
            Task.Run(() => WriteToClient(client, buffer, length));
    }

    public void Dispose()
    {
        listener.Stop();
    }

    private async Task WriteToClient(NetworkStream stream, byte[] buffer, int length)
    {
        try
        {
            await stream.WriteAsync(buffer, 0, length);
        }
        catch
        {
            clients.Remove(stream);
        }
    }

    private void Listen()
    {
        while (true)
        {
            var client = listener.AcceptTcpClient();
            Task.Run(() => HandleClient(client));
        }
    }

    private async Task HandleClient(TcpClient client)
    {
        var stream = client.GetStream();
        clients.Add(stream);
        try
        {
            await ReadClientData(stream);
        }
        finally
        {
            clients.Remove(stream);
            client.Close();
        }
    }

    private async Task ReadClientData(NetworkStream stream)
    {
        var buffer = new byte[4096];
        while (stream.CanRead)
        {
            var result = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (result <= 0)
                return;
            OnRead?.Invoke(buffer, result);
        }
    }
}