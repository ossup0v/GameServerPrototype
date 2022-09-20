using ServerPrototype.Shared;

namespace ServerPrototype.Common.Models
{
    public class PlayerInfo
    {
        public IReadOnlyDictionary<ResourceType, ulong> Resources { get; set; }
        public ConstructionInfo[] Constructions { get; set; }
        public string Nickname { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
