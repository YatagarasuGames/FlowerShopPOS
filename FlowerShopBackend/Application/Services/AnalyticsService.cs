using FlowerShop.Application.DTO;
using FlowerShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Application.Services
{
    public class AnalyticsService
    {
        private readonly FlowersDbContext _context;

        public AnalyticsService(FlowersDbContext context)
        {
            _context = context;
        }

        public async Task<FinancialSummaryDTO> GetFinancialSummaryAsync(DateTime? from, DateTime? to)
        {
            var startDate = from.HasValue
                ? DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Utc)
                : new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var endDate = to.HasValue
                ? DateTime.SpecifyKind(to.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc)
                : DateTime.UtcNow;

            // 1. Оценка текущего остатка склада (по всем активным партиям RemainingQuantity > 0)
            var activeBatches = await _context.ProductBatches
                .Where(b => b.RemainingQuantity > 0)
                .ToListAsync();

            var currentStockPurchaseValue = activeBatches.Sum(b => b.RemainingQuantity * b.PurchasePrice);
            var currentStockRetailValue = activeBatches.Sum(b => b.RemainingQuantity * b.SellingPrice);

            // 2. Продажи за период
            var totalRevenue = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .SumAsync(o => (float?)o.TotalAmount) ?? 0f;

            var costOfGoodsSold = await _context.OrderItems
                .Where(oi => oi.Order.CreatedAt >= startDate && oi.Order.CreatedAt <= endDate)
                .SumAsync(oi => (float?)(oi.Quantity * oi.PurchasePrice)) ?? 0f;

            var totalDiscountsGiven = await _context.OrderItems
                .Where(oi => oi.Order.CreatedAt >= startDate && oi.Order.CreatedAt <= endDate)
                .SumAsync(oi => (float?)(oi.Quantity * (oi.BaseUnitPrice - oi.FinalUnitPrice))) ?? 0f;

            // 3. Поставки за период: затраты на закупку и потенциальная выручка партий
            var periodBatchesFromSupplies = await _context.ProductBatches
                .Where(b => b.ReceivedAt >= startDate && b.ReceivedAt <= endDate)
                .ToListAsync();

            var periodSuppliesCost = periodBatchesFromSupplies.Sum(b => b.InitialQuantity * b.PurchasePrice);
            var periodSuppliesPotential = periodBatchesFromSupplies.Sum(b => b.InitialQuantity * b.SellingPrice);

            // 4. Списания за период: убыток по закупке и потерянная выручка по розничной цене
            var writeOffs = await _context.WriteOffs
                .Where(w => w.CreatedAt >= startDate && w.CreatedAt <= endDate)
                .Include(w => w.Product)
                .ToListAsync();

            var writeOffPurchaseCost = writeOffs.Sum(w => w.Quantity * w.PurchasePriceAtWriteOff);
            var writeOffRetailLoss = writeOffs.Sum(w => w.Quantity * w.Product.CurrentPrice);

            var grossProfit = totalRevenue - costOfGoodsSold;
            var netProfit = grossProfit - writeOffPurchaseCost; // Чистая прибыль: заработок минус потери от брака
            var cashBalance = totalRevenue - periodSuppliesCost; // Денежный остаток: сколько денег осело в кассе

            return new FinancialSummaryDTO
            {
                From = startDate,
                To = endDate,
                CurrentStockPurchaseValue = (float)Math.Round(currentStockPurchaseValue, 2),
                CurrentStockRetailValue = (float)Math.Round(currentStockRetailValue, 2),
                TotalRevenue = (float)Math.Round(totalRevenue, 2),
                CostOfGoodsSold = (float)Math.Round(costOfGoodsSold, 2),
                GrossProfit = (float)Math.Round(grossProfit, 2),
                NetProfit = (float)Math.Round(netProfit, 2),
                CashBalance = (float)Math.Round(cashBalance, 2),
                PeriodSuppliesPurchaseCost = (float)Math.Round(periodSuppliesCost, 2),
                PeriodSuppliesPotentialRevenue = (float)Math.Round(periodSuppliesPotential, 2),
                PeriodWriteOffPurchaseCost = (float)Math.Round(writeOffPurchaseCost, 2),
                PeriodWriteOffRetailLoss = (float)Math.Round(writeOffRetailLoss, 2),
                TotalDiscountsGiven = (float)Math.Round(totalDiscountsGiven, 2)
            };
        }

        public async Task<List<TopProductDTO>> GetTopSellingProductsAsync(DateTime? from, DateTime? to, int limit = 5)
        {
            var startDate = from.HasValue
                ? DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Utc)
                : DateTime.UtcNow.AddDays(-30);

            var endDate = to.HasValue
                ? DateTime.SpecifyKind(to.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc)
                : DateTime.UtcNow;

            return await _context.OrderItems
                .Where(oi => oi.Order.CreatedAt >= startDate && oi.Order.CreatedAt <= endDate)
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
                .Select(g => new TopProductDTO
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenueGenerated = (float)Math.Round(g.Sum(x => x.Quantity * x.FinalUnitPrice), 2)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<AnalyticsReportDTO> GetFullReportAsync(DateTime? from, DateTime? to, int topLimit = 5)
        {
            var summary = await GetFinancialSummaryAsync(from, to);
            var topProducts = await GetTopSellingProductsAsync(from, to, topLimit);

            return new AnalyticsReportDTO
            {
                FinancialSummary = summary,
                TopSellingProducts = topProducts
            };
        }
    }
}