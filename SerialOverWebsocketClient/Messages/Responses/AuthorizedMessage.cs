namespace SerialOverWebsocketClient.Messages.Responses;

class AuthorizedMessage : SessionMessage
{
    public static string DefaultType => "authorized";

    public override string Type => DefaultType;
    public bool Status { get; }
}