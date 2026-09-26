namespace FlowerShop.Application.DTO
{
    public class CreateOrderDTO
    {
        public Guid CashierId { get; set; }
        public List<CreateOrderItemDTO> Items { get; set; } = new();
    }
}
