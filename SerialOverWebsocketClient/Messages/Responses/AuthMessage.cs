namespace SerialOverWebsocketClient.Messages.Responses;

class AuthMessage : SessionMessage
{
    public static string DefaultType => "auth";

    public override string Type => DefaultType;
    public string Token { get; set; }

    public byte[]? GetBytes()
    {
        try
        {
            return Convert.FromBase64String(Token);
        }
        catch
        {
            return null;
        }
    }
}