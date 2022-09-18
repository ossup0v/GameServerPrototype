using ServerPrototype.Core.Models;

namespace ServerPrototype.Core.Data
{
    public class ContentProvider
    {
        private ContentProvider() { }
        public static ContentProvider Instance = new ContentProvider();

        public Dictionary<int, FarmConstruction> GetFarmConstructions()
        {
            return new Dictionary<int, FarmConstruction>
            {
                [0] = new FarmConstruction { Id = 0 },
                [1] = new FarmConstruction { Id = 1 },
            };
        }
    }
}
