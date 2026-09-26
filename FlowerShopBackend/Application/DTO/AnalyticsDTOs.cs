namespace FlowerShop.Application.DTO
{
    public class FinancialSummaryDTO
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        // Общая оценка склада в моменте
        public float CurrentStockPurchaseValue { get; set; } // Себестоимость всего товара на складе прямо сейчас
        public float CurrentStockRetailValue { get; set; }   // Потенциальная выручка при продаже всего склада прямо сейчас

        // Финансы за выбранный период
        public float TotalRevenue { get; set; }              // Выручка
        public float CostOfGoodsSold { get; set; }          // Себестоимость проданного товара (COGS)
        public float GrossProfit { get; set; }              // Валовая прибыль (Выручка - COGS)
        public float TotalDiscountsGiven { get; set; }      // Сумма уценок и скидок

        public float NetProfit { get; set; }                // ЧИСТАЯ ПРИБЫЛЬ (Валовая - Списания)
        public float CashBalance { get; set; }              // Денежный остаток (Выручка - Новые закупки)

        // Поставки за период
        public float PeriodSuppliesPurchaseCost { get; set; } // Сколько потрачено на закупку новых партий
        public float PeriodSuppliesPotentialRevenue { get; set; } // Сколько можно выручить с этих партий

        // Списания за период
        public float PeriodWriteOffPurchaseCost { get; set; } // Убыток по закупке (себестоимость списанного)
        public float PeriodWriteOffRetailLoss { get; set; }   // Потерянная потенциальная выручка по розничным ценам

        public float NetCashFlow { get; set; }              // Денежный поток: Выручка - Закупки - Убытки
    }

    public class AnalyticsReportDTO
    {
        public FinancialSummaryDTO FinancialSummary { get; set; } = new();
        public List<TopProductDTO> TopSellingProducts { get; set; } = new();
    }
}