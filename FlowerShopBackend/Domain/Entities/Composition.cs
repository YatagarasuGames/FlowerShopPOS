namespace FlowerShop.Domain.Entities
{
    public class Composition
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string Name { get; set; } = "Букет";
        public float AssemblyPrice { get; set; }
        public float TotalPrice { get; set; }

        public Order Order { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}