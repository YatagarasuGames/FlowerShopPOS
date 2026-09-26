using FlowerShop.Domain.Enums;

namespace FlowerShop.Application.DTO
{
    public class OrderItemRequestDTO
    {
        public Guid ProductId { get; set; }
        public Guid? ProductBatchId { get; set; }
        public int Quantity { get; set; }
        public DiscountType DiscountType { get; set; } = DiscountType.None;
        public float DiscountValue { get; set; } = 0f;
    }

    public class CompositionRequestDTO
    {
        public string Name { get; set; } = "Сборный букет";
        public float AssemblyPrice { get; set; } = 0f;
        public List<OrderItemRequestDTO> Items { get; set; } = new();
    }

    public class CreateAdvancedOrderDTO
    {
        public List<OrderItemRequestDTO> SingleItems { get; set; } = new();

        public List<CompositionRequestDTO> Compositions { get; set; } = new();
    }

    public class OrderItemReceiptDTO
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public float BaseUnitPrice { get; set; }
        public float FinalUnitPrice { get; set; }
        public float TotalPrice { get; set; }
        public string DiscountInfo { get; set; } = string.Empty;
    }

    public class CompositionReceiptDTO
    {
        public string Name { get; set; } = string.Empty;
        public float AssemblyPrice { get; set; }
        public float TotalPrice { get; set; }
        public List<OrderItemReceiptDTO> Items { get; set; } = new();
    }

    public class AdvancedOrderResponseDTO
    {
        public Guid OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public float TotalAmount { get; set; }
        public List<OrderItemReceiptDTO> SingleItems { get; set; } = new();
        public List<CompositionReceiptDTO> Compositions { get; set; } = new();
    }
}