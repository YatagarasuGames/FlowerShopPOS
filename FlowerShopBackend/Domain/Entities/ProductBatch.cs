namespace FlowerShop.Domain.Entities
{
    public class ProductBatch
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid? SupplyId { get; set; }
        public float PurchasePrice { get; set; }
        public float SellingPrice { get; set; }
        public int InitialQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public DateTime ReceivedAt { get; set; }

        public Product Product { get; set; } = null!;
        public Supply? Supply { get; set; }
    }
}