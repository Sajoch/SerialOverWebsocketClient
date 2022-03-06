namespace SerialOverWebsocketClient.Messages.Responses;

class ReadMessage : SessionMessage
{
    public static string DefaultType => "read";

    public override string Type => DefaultType;
    public string Data { get; set; }
}