using ServerPrototype.DAL.Api;

namespace ServerPrototype.Core.Components
{
    public class InventoryComponent : ComponentBase
    {
        private Dictionary<int, ulong> _resources;
        private readonly IPlayerInventoryDb _inventoryDb;
        public IReadOnlyDictionary<int, ulong> Resources => _resources;

        public InventoryComponent(IPlayerInventoryDb inventoryDb)
        {
            _inventoryDb = inventoryDb;
        }

        public override async Task Init(string owner)
        {
            _resources = await _inventoryDb.GetResources(owner);
            await base.Init(owner);
        }

        public bool CheckIsEnough(int resourceId, ulong amount)
        {
            return _resources.ContainsKey(resourceId) ? _resources[resourceId] >= amount : false;
        }

        public void IncreaseResource(int resourceId, ulong amount)
        {
            if (!_resources.ContainsKey(resourceId))
                _resources.Add(resourceId, 0UL);

            _resources[resourceId] += amount;
        }

        public Task SaveChanges()
        {
            return _inventoryDb.SaveResources(_resources, Owner);
        }

        public bool TryDescreaseResource(int resourceId, ulong amount)
        {
            if (CheckIsEnough(resourceId, amount) is false)
                return false;

            _resources[resourceId] -= amount;

            return true;
        }
    }
}
