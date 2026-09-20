namespace ElectronicsWareHouse.ViewModels
{
    public class AIInventoryDataViewModel
    {
        public List<AIProductViewModel> Products { get; set; }
            = new();

        public List<AILowStockProductViewModel> LowStockProducts { get; set; }
            = new();

        public List<AISalesSummaryViewModel> SalesSummary { get; set; }
            = new();
    }


    public class AIProductViewModel
    {
        public string ProductName { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int StockQuantity { get; set; }

        public int LowStockThreshold { get; set; }
    }


    public class AILowStockProductViewModel
    {
        public string ProductName { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public int StockQuantity { get; set; }

        public int LowStockThreshold { get; set; }
    }


    public class AISalesSummaryViewModel
    {
        public string ProductName { get; set; } = string.Empty;

        public int QuantitySold { get; set; }

        public decimal Revenue { get; set; }
    }
}