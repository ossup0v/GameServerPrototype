using ServerPrototype.Shared;

namespace ServerPrototype.DAL.Api
{
    public interface IInventoryDb
    {
        Task<Dictionary<ResourceType, ulong>> GetResources(string owner);
        Task SaveResources(Dictionary<ResourceType, ulong> resources, string owner);
    }

    public interface IPlayerInventoryDb : IInventoryDb
    {
        
    }
}
