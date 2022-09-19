namespace ServerPrototype.DAL.Api
{
    public interface IInventoryDb
    {
        Task<Dictionary<int, ulong>> GetResources(string owner);
        Task SaveResources(Dictionary<int, ulong> resources, string owner);
    }

    public interface IPlayerInventoryDb : IInventoryDb
    {
        
    }
}
