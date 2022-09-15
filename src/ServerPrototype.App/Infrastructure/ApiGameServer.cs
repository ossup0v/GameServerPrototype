using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using ServerPrototype.App.Configs;
using ServerPrototype.Common.Networking;
using System.Net.Sockets;

namespace ServerPrototype.App.Infrastructure
{
    public class ApiGameServer : TcpServer, IApiGameServer
    {
        private readonly ILogger<ApiGameServer> _log;
        private readonly ObjectPool<UserSession> _pool;
        private readonly IServiceProvider _serviceProvider;
        private long _totalClientConnected;
        private long _totalClientDisconnected;

        public ApiGameServer(IOptions<ApiServerOptions> options, 
            ILogger<ApiGameServer> log, 
            ObjectPool<UserSession> pool, 
            IServiceProvider serviceProvider) : base(options.Value.Address, options.Value.Port)
        {
            _log = log;
            _pool = pool;
            _serviceProvider = serviceProvider;
        }

        protected override TcpSession CreateUserSession()
        {
            //var session = _pool.Get();
            var session = new UserSession();
            session.Init(this, _serviceProvider);
            return session;
        }

        public override bool Start()
        {
            _log.LogInformation("Starting TCP API server");
            return base.Start();
        }

        public GameServerStatistics GetStatistics()
        {
            _totalClientConnected = TotalConnected;
            return new GameServerStatistics
            {
                TotalBytesReceived = BytesReceived,
                TotalBytesSent = BytesSent,
                TotalClientConnected = _totalClientConnected,
                TotalClientDisconnected = TotalDisconnected,
                CurrentClientCount = ConnectedSessions
            };
        }

        public long TotalConnected => _totalClientConnected;
        public long TotalDisconnected => _totalClientDisconnected;

        protected override void OnConnected(TcpSession session)
        {
            var id = session.Id;
            Interlocked.Increment(ref _totalClientConnected);
            _log.LogInformation("Client connected. ObjectId: {id}, ", id);
            base.OnConnected(session);
        }

        protected override void OnDisconnected(TcpSession session)
        {
            var id = session.Id;

            _log.LogInformation("Client disconnected. ObjectId: {id}, ", id);
            Interlocked.Increment(ref _totalClientDisconnected);

            session.Dispose();
            //_pool.Return((UserSession)session);

            base.OnDisconnected(session);
        }

        protected override void OnStarted()
        {
            _log.LogInformation("TCP API Server started");
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            _log.LogInformation("TCP API Server stopped");
            base.OnStopped();
        }

        protected override void OnError(SocketError error)
        {
            _log.LogError("SocketError: {error}", error.ToString());
            base.OnError(error);
        }
    }
}
