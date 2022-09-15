using MessagePack;

namespace ServerPrototype.Shared.Packets.ServerToClient
{

    [MessagePackObject]
    public class LoginConfirmPacket : PlayerProfileInfo, IServerToClientPacket
    {
        protected bool Equals(LoginConfirmPacket other)
        {
            return base.Equals(other);
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
            return Equals((LoginConfirmPacket)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(LoginConfirmPacket left, LoginConfirmPacket right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LoginConfirmPacket left, LoginConfirmPacket right)
        {
            return !Equals(left, right);
        }
    }

    public interface INetworkPlayer
    {
        string UserId { get; }
        string NickName { get; set; }
    }
}
