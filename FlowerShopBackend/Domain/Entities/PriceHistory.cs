namespace FlowerShop.Domain.Entities
{
    public class PriceHistory
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public float OldPrice { get; set; }
        public float NewPrice { get; set; }
        public Guid ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }

        public Product Product { get; set; }
        public User Cashier { get; set; }
    }
}
