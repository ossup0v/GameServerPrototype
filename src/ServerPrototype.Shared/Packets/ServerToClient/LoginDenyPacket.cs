using MessagePack;

namespace ServerPrototype.Shared.Packets.ServerToClient
{
    [MessagePackObject]
    public class LoginDenyPacket : IServerToClientPacket
    {
        [Key(0)]
        public string Reason { get; set; }

        protected bool Equals(LoginDenyPacket other)
        {
            return Reason == other.Reason;
        }

        public bool Equals(IServerToClientPacket other)
        {
            return Equals(other as object);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LoginDenyPacket)obj);
        }

        public override int GetHashCode()
        {
            return (Reason != null ? Reason.GetHashCode() : 0);
        }
    }
}
