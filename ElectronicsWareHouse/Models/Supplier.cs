using System.ComponentModel.DataAnnotations;

namespace ElectronicsWareHouse.Models
{
    public class Supplier
    {
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [StringLength(150)]
        public string SupplierName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ContactName { get; set; }

        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }

        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}