namespace ServerPrototype.App.Infrastructure
{
    public interface IApiGameServer
    {
        bool Start();
        bool Stop();
        GameServerStatistics GetStatistics();
    }
}
