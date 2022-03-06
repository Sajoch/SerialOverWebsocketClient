namespace SerialOverWebsocketClient.Messages.Requests;

class WriteMessage : SessionMessage
{
    public override string Type => "write";
    public string Data { get; set; }
}