namespace FlowerShop.Application.DTO
{
    public class OrderResponseDTO
    {
        public Guid Id { get; set; }
        public Guid CashierId { get; set; }
        public DateTime CreatedAt { get; set; }
        public float TotalAmount { get; set; }
        public List<OrderItemResponseDTO> Items { get; set; } = new();
    }
}
