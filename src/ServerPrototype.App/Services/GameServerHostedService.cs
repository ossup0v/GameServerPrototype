using ServerPrototype.App.Infrastructure;

namespace ServerPrototype.App.Services
{
    public class GameServerHostedService : IHostedService
    {
        private readonly ILogger<GameServerHostedService> _log;
        private readonly IApiGameServer _gameServer;

        public GameServerHostedService(ILogger<GameServerHostedService> log, IApiGameServer gameServer)
        {
            _log = log;
            _gameServer = gameServer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _gameServer.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _gameServer.Stop();
            return Task.CompletedTask;
        }
    }
}
