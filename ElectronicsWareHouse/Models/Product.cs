using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ElectronicsWareHouse.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters.")]
        [Display(Name = "SKU Code")]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(150, ErrorMessage = "Product Name cannot exceed 150 characters.")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category selection is required.")]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Unit Price is required.")]
        [Range(0.01, 1000000.00, ErrorMessage = "Unit Price must be greater than 0.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Stock Quantity is required.")]
        [Range(0, 1000000, ErrorMessage = "Stock Quantity cannot be negative.")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Low Stock Threshold is required.")]
        [Range(0, 100000, ErrorMessage = "Low Stock Threshold cannot be negative.")]
        [Display(Name = "Low Stock Threshold")]
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