using MessagePack;

namespace ServerPrototype.Shared.Packets.ClientToServer
{
    /// <summary><see cref="ClientToServerPacketType.Login"/></summary>
    [MessagePackObject]
    public class LoginPacket : IClientToServerPacket
    {
        public LoginPacket() { }

        public LoginPacket(string clientId)
        {
            ClientId = clientId;
        }

        [Key(0)]
        public string ClientId { get; set; }

        public override string ToString()
        {
            return $"{GetType().Name} {nameof(ClientId)}: {ClientId}";
        }

        public bool Equals(IClientToServerPacket other)
        {
            var packet = other as LoginPacket;
            if (packet == null)
                return false;
            return ClientId == packet.ClientId;
        }
    }
}
