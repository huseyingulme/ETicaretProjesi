namespace ETicaret.Core.Models
{
    public class ProductStatistics
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int InactiveProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int FeaturedProducts { get; set; }
        public int ProductsWithDiscount { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal HighestPrice { get; set; }
        public decimal LowestPrice { get; set; }
        public decimal TotalStockValue { get; set; }
        public int TotalCategories { get; set; }
        public int TotalBrands { get; set; }
        public Dictionary<string, int> ProductsByCategory { get; set; } = new();
        public Dictionary<string, int> ProductsByBrand { get; set; } = new();
        public Dictionary<string, decimal> SalesByCategory { get; set; } = new();
        public Dictionary<string, decimal> SalesByBrand { get; set; } = new();
        public List<TopSellingProduct> TopSellingProducts { get; set; } = new();
        public List<LowStockProduct> LowStockProductsList { get; set; } = new();
    }

    public class TopSellingProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int SalesCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
    }

    public class LowStockProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
    }
}
