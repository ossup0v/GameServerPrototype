using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ServerPrototype.Common.Models;
using ServerPrototype.DAL.Api;
using ServerPrototype.DAL.Configs;
using ServerPrototype.DAL.Models;

namespace ServerPrototype.DAL.Implementation
{
    public class PlayerEffectsDb : IPlayerEffectsDb
    {
        private IMongoCollection<EffectEntity> _inventory;
        public PlayerEffectsDb(IOptions<PlayerEffectDbConfig> config)
        {
            MongoClient client = new MongoClient(config.Value.ConnectionString);
            IMongoDatabase db = client.GetDatabase(config.Value.DbName);
            _inventory = db.GetCollection<EffectEntity>(config.Value.CollectionName);
        }

        public async Task<Dictionary<EffectKind, Effect>> GetEffects(string owner)
        {
            return (await _inventory.FindAsync(x => x.Id == owner)).FirstOrDefault()?.Effects ?? new Dictionary<EffectKind, Effect>();
        }

        public Task SaveEffects(Dictionary<EffectKind, Effect> effects, string owner)
        {
            return _inventory.ReplaceOneAsync(x => x.Id == owner, new EffectEntity { Id = owner, Effects = effects }, new ReplaceOptions { IsUpsert = true });
        }
    }
}
