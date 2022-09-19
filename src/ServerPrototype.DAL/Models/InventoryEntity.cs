using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ServerPrototype.DAL.Models
{
    public class InventoryEntity
    {
        [BsonId]
        public string Id { get; set; }

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, ulong> Resources { get; set; }
    }
}
