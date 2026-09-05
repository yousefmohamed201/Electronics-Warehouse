using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ElectronicsWareHouse.Models
{
    public class PurchaseItem
    {
        public int PurchaseItemID { get; set; }

        public int PurchaseID { get; set; }

        [Required(ErrorMessage = "Please select a product.")]
        public int ProductID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Unit cost cannot be negative.")]
        public decimal UnitCost { get; set; }

        [ValidateNever]
        public Purchase Purchase { get; set; } = null!;

        [ValidateNever]
        public Product Product { get; set; } = null!;
    }
}