namespace ServerPrototype.Shared.Packets.ServerToClient
{
    public sealed class GenericErrorPacket : IServerToClientPacket
    {
        public string Error { get; set; }
        public ulong RequestNum { get; set; }

        public bool Equals(IServerToClientPacket other)
        {
            return Equals(other as object);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GenericErrorPacket)obj);
        }
    }
}
