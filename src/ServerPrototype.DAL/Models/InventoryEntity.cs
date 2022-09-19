using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using ServerPrototype.Shared;

namespace ServerPrototype.DAL.Models
{
    public class InventoryEntity
    {
        [BsonId]
        public string Id { get; set; }

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<ResourceType, ulong> Resources { get; set; }
    }
}
