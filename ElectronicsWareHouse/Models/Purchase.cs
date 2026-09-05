using System.ComponentModel.DataAnnotations;

namespace ElectronicsWareHouse.Models
{
    public class Purchase
    {
        public int PurchaseID { get; set; }

        [Required(ErrorMessage = "Please select a supplier.")]
        public int SupplierID { get; set; }

        public DateTime PurchaseDate { get; set; }

        public decimal TotalAmount { get; set; }

        public Supplier Supplier { get; set; } = null!;

        public ICollection<PurchaseItem> PurchaseItems { get; set; }
            = new List<PurchaseItem>();
    }
}