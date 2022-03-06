namespace SerialOverWebsocketClient.Messages.Requests;

class ResponseAuthMessage : SessionMessage
{
    public override string Type => "response-auth";
    public string Token { get; set; }

    public ResponseAuthMessage(byte[] hash)
    {
        Token = Convert.ToBase64String(hash);
    }
}