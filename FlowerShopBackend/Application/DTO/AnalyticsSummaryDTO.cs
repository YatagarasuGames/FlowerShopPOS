namespace FlowerShop.Application.DTO
{
    public class AnalyticsSummaryDTO
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCostOfGoodsSold { get; set; }
        public decimal TotalWriteOffCost { get; set; }
        public decimal NetProfit { get; set; }
        public int OrdersCount { get; set; }
        public decimal AverageCheck { get; set; }
    }
}
