using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ServerPrototype.DAL.Api;
using ServerPrototype.DAL.Configs;
using ServerPrototype.DAL.Models;
using ServerPrototype.Shared;

namespace ServerPrototype.DAL.Implementation
{
    public class PlayerInventoryDb : IPlayerInventoryDb
    {
        private IMongoCollection<InventoryEntity> _inventory;
        public PlayerInventoryDb(IOptions<PlayerInventoryDbConfig> config)
        {
            MongoClient client = new MongoClient(config.Value.ConnectionString);
            IMongoDatabase db = client.GetDatabase(config.Value.DbName);
            _inventory = db.GetCollection<InventoryEntity>(config.Value.CollectionName);
        }

        public async Task<Dictionary<ResourceType, ulong>> GetResources(string owner)
        {
            try
            {
                var coursor = await _inventory.FindAsync(x => x.Id == owner);
                var entity = coursor.FirstOrDefault();
                return entity?.Resources ?? new Dictionary<ResourceType, ulong>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task SaveResources(Dictionary<ResourceType, ulong> resources, string owner)
        {
            return _inventory.ReplaceOneAsync(x => x.Id == owner, new InventoryEntity { Id = owner, Resources = resources }, new ReplaceOptions { IsUpsert = true });
        }
    }
}
