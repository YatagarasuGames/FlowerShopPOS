namespace FlowerShop.Domain.Entities
{
    public class Supply
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ReceivedByUserId { get; set; }
        public int Quantity { get; set; }
        public float PurchasePrice { get; set; }
        public DateTime CreatedAt { get; set; }

        public Product Product { get; set; }
        public User ReceivedBy { get; set; }
    }
}
