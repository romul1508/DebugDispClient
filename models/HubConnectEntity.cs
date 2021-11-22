namespace DebugOmgDispClient.models
{
    public record HubConnectEntity: BaseEntity
    {
        public string DispName { get; set; }
        public string IpAddr { get; set; }
        public string ConnectId { get; set; }
    }
}
