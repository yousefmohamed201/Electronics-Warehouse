namespace ElectronicsWareHouse.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        public string SKU { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public int CategoryID { get; set; }

        public decimal UnitPrice { get; set; }

        public int StockQuantity { get; set; }

        public int LowStockThreshold { get; set; }

        // Relationship with Category
        public Category Category { get; set; } = null!;

        // Relationship with PurchaseItems
        public ICollection<PurchaseItem> PurchaseItems { get; set; }
            = new List<PurchaseItem>();

        // Relationship with SaleItems
        public ICollection<SaleItem> SaleItems { get; set; }
            = new List<SaleItem>();
    }
}