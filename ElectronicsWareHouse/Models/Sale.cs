using System.Net.ServerSentEvents;

namespace ElectronicsWareHouse.Models
{
    public class Sale
    {
        public int SaleID { get; set; }

        public DateTime SaleDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string? CustomerInfo { get; set; }

        public ICollection<SaleItem> SaleItems { get; set; }
            = new List<SaleItem>();
    }
}
