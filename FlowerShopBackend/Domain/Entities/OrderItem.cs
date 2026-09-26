using FlowerShop.Domain.Enums;

namespace FlowerShop.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid? CompositionId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductBatchId { get; set; }

        public int Quantity { get; set; }
        public float PurchasePrice { get; set; }
        public float BaseUnitPrice { get; set; }

        public DiscountType DiscountType { get; set; } = DiscountType.None;
        public float DiscountValue { get; set; }
        public float FinalUnitPrice { get; set; }
        public float TotalPrice { get; set; }

        public Order Order { get; set; } = null!;
        public Composition? Composition { get; set; }
        public Product Product { get; set; } = null!;
        public ProductBatch ProductBatch { get; set; } = null!;
    }
}