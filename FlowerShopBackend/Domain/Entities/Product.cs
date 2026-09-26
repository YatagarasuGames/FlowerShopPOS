namespace FlowerShop.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float CurrentPrice { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
    }
}
