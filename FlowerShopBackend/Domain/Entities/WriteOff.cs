namespace FlowerShop.Domain.Entities
{
    public class WriteOff
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductBatchId { get; set; }
        public Guid CashierId { get; set; }
        public int Quantity { get; set; }
        public float PurchasePriceAtWriteOff { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public Product Product { get; set; } = null!;
        public ProductBatch ProductBatch { get; set; } = null!;
        public User Cashier { get; set; } = null!;
    }
}