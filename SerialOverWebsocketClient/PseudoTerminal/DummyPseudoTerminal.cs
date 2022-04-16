namespace SerialOverWebsocketClient.PseudoTerminal;

class DummyPseudoTerminal : IPseudoTerminal
{
    public event Action<byte[], int> OnRead;

    public string Filename => "";

    public void Start(int index)
    {
    }

    public void Write(byte[] buffer, int length)
    {
    }

    public void Dispose()
    {
    }
}