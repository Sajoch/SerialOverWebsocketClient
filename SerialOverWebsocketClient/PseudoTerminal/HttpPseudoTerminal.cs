using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SerialOverWebsocketClient.PseudoTerminal;

public class HttpPseudoTerminal : IPseudoTerminal
{
    public event Action<byte[], int> OnRead;
    private readonly HttpListener listener = new();
    public string Filename { get; private set; }

    public void Start(int index)
    {
        var port = 8000 + index;
        Filename = $"http://localhost:{port}/";
        listener.Prefixes.Add($"http://*:{port}/");
        listener.Start();

        Task.Run(Listen);
    }

    public void Write(byte[] buffer, int length)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        listener.Stop();
    }
    private void Listen()
    {
        while (true)
        {
            var client = listener.GetContext();
            Task.Run(() => HandleClient(client));
        }
    }

    private void HandleClient(HttpListenerContext client)
    {
        var path = client.Request.Url.AbsolutePath;
        
        client.Response.StatusCode = 500;
        if (path.StartsWith("/"))
        {
            var bytes = Encoding.UTF8.GetBytes(path.Skip(1).ToArray());
            OnRead?.Invoke(bytes, bytes.Length);
            client.Response.StatusCode = 200;
        }

        client.Response.Close();
    }
}