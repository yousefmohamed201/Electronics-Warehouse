using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ElectronicsWareHouse.Models
{
    public class Purchase
    {
        public int PurchaseID { get; set; }

        [Required(ErrorMessage = "Please select a supplier.")]
        public int SupplierID { get; set; }

        public DateTime PurchaseDate { get; set; }

        public decimal TotalAmount { get; set; }

        [ValidateNever]
        public Supplier Supplier { get; set; } = null!;

        [ValidateNever]
        public ICollection<PurchaseItem> PurchaseItems { get; set; }
            = new List<PurchaseItem>();
    }
}