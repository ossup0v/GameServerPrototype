using ServerPrototype.DAL.Api;
using ServerPrototype.Shared;
using static MongoDB.Driver.WriteConcern;

namespace ServerPrototype.Core.Components
{
    public class InventoryComponent : ComponentBase
    {
        private Dictionary<ResourceType, ulong> _resources;
        private readonly IPlayerInventoryDb _inventoryDb;
        public IReadOnlyDictionary<ResourceType, ulong> Resources => _resources;

        public InventoryComponent(IPlayerInventoryDb inventoryDb)
        {
            _inventoryDb = inventoryDb;
        }

        public override async Task Init(string owner)
        {
            _resources = await _inventoryDb.GetResources(owner);
            await base.Init(owner);
        }

        public bool CheckResourceIsEnough(ResourceType resource, ulong amount)
        {
            return _resources.ContainsKey(resource) ? _resources[resource] >= amount : false;
        }

        public void IncreaseResource(ResourceType resource, ulong amount)
        {
            if (!_resources.ContainsKey(resource))
                _resources.Add(resource, 0UL);

            _resources[resource] += amount;
        }

        public Task SaveChanges()
        {
            return _inventoryDb.SaveResources(_resources, Owner);
        }

        public bool TryDescreaseResource(ResourceType resource, ulong amount)
        {
            if (CheckResourceIsEnough(resource, amount) is false)
                return false;

            _resources[resource] -= amount;

            return true;
        }

        public bool TryDescreaseResource(Dictionary<ResourceType, ulong> resources)
        {
            foreach (var (resource, amount) in resources)
                if (CheckResourceIsEnough(resource, amount) is false)
                    return false;
            
            foreach (var (resource, amount) in resources)
                _resources[resource] -= amount;

            return true;
        }
    }
}
