namespace SerialOverWebsocketClient.PseudoTerminal;

public interface IPseudoTerminal : IDisposable
{
    event Action<byte[], int> OnRead;
    string Filename { get; }
    void Write(byte[] buffer, int length);
}