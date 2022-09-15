namespace ServerPrototype.App.Infrastructure
{
    public sealed class GameServerStatistics
    {
        public long TotalBytesReceived { get; set; }
        public long TotalBytesSent { get; set; }
        public long TotalClientConnected { get; set; }
        public long TotalClientDisconnected { get; set; }
        public long CurrentClientCount { get; set; }
    }
}
