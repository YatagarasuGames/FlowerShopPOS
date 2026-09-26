namespace FlowerShop.Application.DTO
{
    public class CreateWriteOffDTO
    {
        public Guid ProductId { get; set; }
        public Guid? ProductBatchId { get; set; }
        public Guid CashierId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
