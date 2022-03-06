namespace SerialOverWebsocketClient.Messages.Requests;

class RequestAuthMessage : SessionMessage
{
    public override string Type => "request-auth";
}