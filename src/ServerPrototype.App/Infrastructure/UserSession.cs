using App.Metrics;
using AutoMapper;
using MessagePack;
using Orleans;
using Orleans.Streams;
using ServerPrototype.Actors.Grains.Interfaces;
using ServerPrototype.Actors.Grains.Messages.Requests;
using ServerPrototype.Common.Networking;
using ServerPrototype.Shared;
using ServerPrototype.Shared.Packets.ClientToServer;
using ServerPrototype.Shared.Packets.ServerToClient;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;

namespace ServerPrototype.App.Infrastructure
{

    public class UserSession : TcpSession
    {
        private IClusterClient _cluster;
        private ILogger<UserSession> _log;

        private Pipe _pipe;
        private CancellationTokenSource _closeTcs;
        private string _userId;
        private IMetricsUpdater _metrics;
        private string _nickname;
        private IAsyncStream<IServerToClientPacket> _personalNotificationsStream;
        private IAsyncStream<IServerToClientPacket> _groupNotificationsStream;
        private IMapper _mapper;
        private StreamSubscriptionHandle<IServerToClientPacket> _groupNotificationHandle;

        public UserSession() : base()
        {
        }

        protected override void OnReceived(byte[] inBuffer, long offset, long size)
        {
            try
            {
                lock (_pipe.Writer)
                {
                    if (IsConnected && !_closeTcs.IsCancellationRequested)
                        _pipe.Writer.AsStream().Write(inBuffer, (int)offset, (int)size);
                }
            }
            catch (Exception e)
            {
                _log.LogError(e, "Can't write to pipe stream");
            }

            base.OnReceived(inBuffer, offset, size);
        }

        private async Task ListenForNewPackets()
        {
            await using var stream = _pipe.Reader.AsStream();
            using var streamReader = new MessagePackStreamReader(stream);

            var packetClose = new CancellationTokenSource();
            var invalidCount = 0;

            while (!_closeTcs.IsCancellationRequested && !packetClose.IsCancellationRequested)
            {
                try
                {
                    while (!_closeTcs.IsCancellationRequested && await streamReader.ReadAsync(_closeTcs.Token) is ReadOnlySequence<byte> msgpack)
                    {
                        using var _ = _log.WithScope(("user_id", _userId), ("nickname", _nickname));

                        if (msgpack.Length == 1)
                        {
                            _log.LogWarning($"Empty packet: {msgpack.FirstSpan[0]}. Maybe message is not in union ;)");
                            break;
                        }

                        var serverPacket = MessagePackSerializer.Deserialize<IClientToServerPacket>(msgpack);

                        if (serverPacket == null)
                        {
                            _log.LogError($"serverPacket is null! First byte: {msgpack.FirstSpan[0]}");
                            continue;
                        }

                        var name = serverPacket.GetType().ToString();
                        _metrics.Increment(name, Unit.Requests);
                        _log.LogTrace("RECV [{0}] ({1}) <{2}>: `{3}`", _nickname, Id, name, serverPacket);
                        using var __ = _metrics.Timing(name);

                        try
                        {
                            switch (serverPacket)
                            {
                                case LoginPacket p:
                                    await OnLogin(p);
                                    break;
                                case StartBuildFarmConstructionPacket p:
                                    await OnStartBuildFarmConstruction(p);
                                    break;
                                default:
                                    _log.LogError("UserSession have no implementation. Packet: {@packet}", serverPacket);
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            _log.LogError(e, "Packet processing failed. Packet: {@packet}", serverPacket);
                            SendGenericError($"Processing `{name}` packet is failed. Message: {e.Message}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    //THIS IS NORMAL
                    break;
                }
                catch (Exception e)
                {
                    _log.LogError(e, $"Error during parsing stream {Id}, {nameof(_nickname)} {_nickname}");
                    invalidCount++;
                    if (invalidCount >= 5)
                    {
                        _log.LogWarning($"Exception is more than 5 times occurs. Disconnecting user. User id: `{_userId}`. Session id: `{Id}`");
                        Disconnect();
                        return;
                    }
                }
            }

            _log.LogInformation("End of ListenForNewPackets");
        }

        private async Task OnLogin(LoginPacket loginRequest)
        {
            using var _ = _log.WithScope(("session_id", Id), ("client_id", loginRequest.ClientId));

            if (!string.IsNullOrEmpty(_userId))
            {
                _log.LogWarning($"Already logged in with id `{_userId}`");
                //SendMessage(new LoginDenyPacket() { Reason = "Already logged in" });
                //return;
            }

            try
            {
                var grain = _cluster.GetGrain<IAccountGrain>(loginRequest.ClientId);
                var apiResult = await grain.Login(LoginRequest.Instance);
                if (apiResult.Status != HttpStatusCode.OK)
                {
                    var message = apiResult.Message;

                    _log.LogWarning("error during login user. message: {message}", message);
                    SendMessage(new LoginDenyPacket() { Reason = message });
                    return;
                }
                else
                {
                    _userId = apiResult.Value.UserId;

                    var playerGrain = _cluster.GetGrain<IPlayerGrain>(_userId);
                    var playerData = await playerGrain.GetPlayerData();
                    if (playerData.Status != HttpStatusCode.OK)
                    {
                        _log.LogError($"Can't get player data. Message: {playerData.Message}");
                        SendMessage(new LoginDenyPacket() { Reason = playerData.Message });
                        return;
                    }

                    await playerGrain.LoginNotify();
                    var player = playerData.Value;

                    SendMessage(new LoginConfirmPacket() { NickName = player.Nickname, UserId = _userId, Heroes = new int[0] });
                    _log.LogInformation($"Loggined. Account: {loginRequest.ClientId}. UserId: {apiResult.Value.UserId}. Nickname: {player.Nickname}. Created: {player.CreatedAt}");

                    _nickname = player.Nickname;

                    //var provider = _cluster.GetStreamProvider(Constants.STORAGE_STREAM_PROVIDER);
                    //_personalNotificationsStream = provider.GetStream<IServerToClientPacket>(Guid.Parse(_userId), Constants.CLIENT_STREAM);
                    //await _personalNotificationsStream.SubscribeAsync((clientPacket, _) =>
                    //{
                    //    SendMessage(clientPacket);
                    //    return Task.CompletedTask;
                    //});
                }
            }
            catch (Exception e)
            {
                _log.LogError(e, "exception during login user");
            }
        }

        private async Task OnStartBuildFarmConstruction(StartBuildFarmConstructionPacket p)
        {
            using var _ = _log.WithScope(("session_id", Id), ("user_id", _userId));

            try
            {
                var playerGrain = _cluster.GetGrain<IPlayerGrain>(_userId);
                var playerData = await playerGrain.StartBuildConstruction(new StartBuildRequest(p.Point, p.ConstructionId));

                if (playerData.Status != HttpStatusCode.OK)
                {
                    _log.LogError($"Can't build farm construction. Message: {playerData.Message}");
                    return;
                }
            }
            catch (Exception e)
            {
                _log.LogError(e, "exception during start building farm construction");
            }
        }


        private async Task TryUnsubscribeFromGroup()
        {
            _log.LogInformation("TryUnsubscribeFromGroup called");

            if (_groupNotificationHandle != null)
            {
                try
                {
                    _log.LogInformation("Unsubscribing from group");
                    await _groupNotificationHandle.UnsubscribeAsync();
                }
                catch (Exception e)
                {
                    _log.LogError(e, "Error during unsubscribing from group notifications");
                }
            }
        }

        private bool SendUnauthorizedErrorIfNeeded()
        {
            if (!string.IsNullOrEmpty(_userId))
                return false;

            _log.LogWarning($"User is not logged in");
            SendGenericError("User is not logged in");
            return true;
        }

        public void Init(IApiGameServer server, IServiceProvider provider)
        {
            try
            {
                _closeTcs?.Cancel(true);
            }
            catch (Exception e)
            {
                _log.LogError(e, "Error during init session");
            }

            try
            {
                _pipe?.Reader.Complete();
                _pipe?.Writer.Complete();
            }
            catch (Exception e)
            {
                _log.LogError(e, $"Can't complete pipe. {Id}");
            }

            _cluster = provider.GetRequiredService<IClusterClient>();
            _log = provider.GetService<ILogger<UserSession>>();
            _closeTcs = new CancellationTokenSource();
            _metrics = provider.GetRequiredService<IMetricsUpdater>();
            _nickname = string.Empty;
            _mapper = provider.GetRequiredService<IMapper>();

            _pipe = new Pipe();
            _userId = string.Empty;

            base.Init((TcpServer)server);
        }

        protected override async void OnConnected()
        {
            _log.LogDebug("Connected Hash: {hash}", GetHashCode());

            _metrics.Increment("client_connected", Unit.Connections);
            _metrics.Increment("client_connections", Unit.Connections);

            await ListenForNewPackets();

            base.OnConnected();
        }

        protected override async void OnDisconnected()
        {
            try
            {
                if (_personalNotificationsStream != null)
                {
                    var handles = await _personalNotificationsStream.GetAllSubscriptionHandles();
                    foreach (var handle in handles)
                    {
                        await handle.UnsubscribeAsync();
                    }
                }
            }
            catch (Exception e)
            {
                _log.LogError(e, "Error during unsubscribing from notifications");
            }

            await TryUnsubscribeFromGroup();

            try
            {
                _closeTcs.Cancel(true);
                await _pipe.Reader.CompleteAsync();
                await _pipe.Writer.CompleteAsync();
            }
            catch (Exception e)
            {
                _log.LogError(e, "Can't complete pipe");
            }

            _metrics.Increment("client_disconnected", Unit.Connections);
            _metrics.Decrement("client_connections", Unit.Connections);

            _userId = null;

            //TODO: think about clearing buffers and decrease size of them
            base.OnDisconnected();
        }

        public void SendMessage(IServerToClientPacket obj)
        {
            using var _ = _log.WithScope(("session_id", Id), ("user_id", _userId));

            var type = obj.GetType().ToString();

            _log.LogTrace($"[{_nickname}] Sending message {type} userId: {_userId}. Message: {obj}");

            _metrics.Increment(type, Unit.Requests);
            byte[] payload = MessagePackSerializer.Serialize(obj);
            SendAsync(payload);
        }

        private void SendGenericError(string error, ulong requestNum = 0)
        {
            SendMessage(new GenericErrorPacket { Error = error, RequestNum = requestNum });
        }
    }
}
