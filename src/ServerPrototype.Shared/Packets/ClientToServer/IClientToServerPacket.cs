using MessagePack;

namespace ServerPrototype.Shared.Packets.ClientToServer
{
    [Union(0, typeof(LoginPacket))]
    public interface IClientToServerPacket : IEquatable<IClientToServerPacket>
    {
        string ToString();
    }
}
