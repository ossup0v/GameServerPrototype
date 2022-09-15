using MessagePack;

namespace ServerPrototype.Shared.Packets.ServerToClient
{
    //[MessagePackObject]
    //public abstract class ProfileInfo : IEquatable<ProfileInfo>
    //{
    //    [Key(0)]
    //    public string NickName { get; set; }
    //    [Key(1)]
    //    public int AvatarId { get; set; }
    //    [Key(2)]
    //    public int WantedHeroId { get; set; }
    //    // todo receive from meta on server instead
    //    [Key(3)]
    //    public int WantedHeroLevel { get; set; }
    //
    //    public bool Equals(ProfileInfo other)
    //    {
    //        if (ReferenceEquals(null, other)) return false;
    //        if (ReferenceEquals(this, other)) return true;
    //        return NickName == other.NickName
    //            && AvatarId == other.AvatarId
    //            && WantedHeroId == other.WantedHeroId
    //            && WantedHeroLevel == other.WantedHeroLevel;
    //    }
    //
    //    public override bool Equals(object obj)
    //    {
    //        if (ReferenceEquals(null, obj)) return false;
    //        if (ReferenceEquals(this, obj)) return true;
    //        if (obj.GetType() != this.GetType()) return false;
    //        return Equals((ProfileInfo)obj);
    //    }
    //
    //    public override int GetHashCode()
    //    {
    //        return HashCode.Combine(NickName, AvatarId, WantedHeroId, WantedHeroLevel);
    //    }
    //
    //    public override string ToString()
    //    {
    //        return $"{GetType().Name} {nameof(NickName)}: {NickName} {nameof(AvatarId)}: {AvatarId} {nameof(WantedHeroId)}: {WantedHeroId} {nameof(WantedHeroLevel)}: {WantedHeroLevel}";
    //    }
    //}
}
