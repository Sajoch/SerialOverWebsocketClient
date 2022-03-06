namespace SerialOverWebsocketClient.Messages.Responses;

class StatusMessage : SessionMessage
{
    public static string DefaultType => "response";

    public override string Type => DefaultType;
    public string Action { get; set; }
    public bool Status { get; set; }
}