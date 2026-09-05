using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
        [ValidateNever]
        public Category Category { get; set; } = null!;

        // Relationship with PurchaseItems
        [ValidateNever]
        public ICollection<PurchaseItem> PurchaseItems { get; set; }
            = new List<PurchaseItem>();

        // Relationship with SaleItems
        [ValidateNever]
        public ICollection<SaleItem> SaleItems { get; set; }
            = new List<SaleItem>();
    }
}