namespace ServerPrototype.Core.Models
{
    public class Item
    {
        public int Id { get; set; }
        public Dictionary<int, Effect> Effects { get; set; }
    }
}
