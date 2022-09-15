using MessagePack;

namespace ServerPrototype.Shared.Packets.ServerToClient
{
    [MessagePackObject]
    [Union(0, typeof(LoginConfirmPacket))]
    //[Union(1, typeof(ProfileInfo))]
    public abstract class PlayerProfileInfo : /*ProfileInfo,*/ INetworkPlayer
    {
        [Key(4)]
        public string UserId { get; set; }

        /// <summary>
        /// Array of ids of available characters (generals) for player
        /// </summary>
        [Key(5)]
        public int[] Heroes { get; set; }
        [Key(6)]
        public string NickName { get; set; }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != this.GetType()) return false;
            return Equals((PlayerProfileInfo)other);

        }
        public bool Equals(PlayerProfileInfo other)
        {
            return UserId == other.UserId && Heroes != null && Heroes.SequenceEqual(other.Heroes);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), UserId, Heroes);
        }

        public override string ToString()
        {
            return base.ToString() + $"{nameof(UserId)}: {UserId}, {nameof(Heroes)}: {string.Join(" ", Heroes)}";
        }
    }
}
