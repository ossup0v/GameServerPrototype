namespace ServerPrototype.Common.Models
{
    public class Item
    {
        public int Id { get; set; }
        public Dictionary<int, Effect> Effects { get; set; }
    }
}
