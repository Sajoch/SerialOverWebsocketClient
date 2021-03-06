namespace SerialOverWebsocketClient.PseudoTerminal;

public interface IPseudoTerminal : IDisposable
{
    event Action<byte[], int> OnRead;
    string Filename { get; }

    void Start(int index);
    void Write(byte[] buffer, int length);
}