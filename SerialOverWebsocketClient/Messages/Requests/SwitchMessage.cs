namespace SerialOverWebsocketClient.Messages.Requests;

class SwitchMessage : SessionMessage
{
    public override string Type =>"switch";
    public bool Data { get; set; }
}