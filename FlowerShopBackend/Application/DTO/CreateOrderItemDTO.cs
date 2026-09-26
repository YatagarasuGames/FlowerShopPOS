namespace FlowerShop.Application.DTO
{
    public class CreateOrderItemDTO
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
