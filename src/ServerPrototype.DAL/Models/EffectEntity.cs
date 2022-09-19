using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using ServerPrototype.Common.Models;
using MongoDB.Bson.Serialization.Options;

namespace ServerPrototype.DAL.Models
{
    public class EffectEntity
    {
        [BsonId]
        public string Id { get; set; }

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<EffectType, Effect> Effects { get; set; }
    }
}
