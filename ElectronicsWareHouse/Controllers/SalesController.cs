using ElectronicsWareHouse.Data;
using ElectronicsWareHouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsWareHouse.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales
                .ToListAsync();
            return View(sales);
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var sale = await _context.Sales
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(s => s.SaleID == id);

            if (sale == null)
                return NotFound();

            return View(sale);
        }

        // GET: Sales/Create
        public async Task<IActionResult> Create()
        {
            await LoadProducts();

            return View();
        }

        // POST: Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Sale sale,
            List<SaleItem> items)
        {
            if (items == null || !items.Any())
            {
                ModelState.AddModelError(
                    "",
                    "Sale must contain at least one product.");
            }

            if (!ModelState.IsValid)
            {
                await LoadProducts();
                return View(sale);
            }

            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                sale.SaleDate = DateTime.Now;
                sale.TotalAmount = 0;

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();

                decimal total = 0;

                foreach (var item in items)
                {
                    if (item.Quantity <= 0)
                        throw new Exception(
                            "Quantity must be greater than zero.");

                    if (item.UnitPrice < 0)
                        throw new Exception(
                            "Unit price cannot be negative.");

                    var product = await _context.Products
                        .FirstOrDefaultAsync(
                            p => p.ProductID == item.ProductID);

                    if (product == null)
                        throw new Exception("Product not found.");

                    // Check Stock
                    if (product.StockQuantity < item.Quantity)
                    {
                        throw new Exception(
                            $"Not enough stock for product: {product.ProductName}");
                    }

                    item.SaleID = sale.SaleID;

                    total += item.Quantity * item.UnitPrice;

                    // Decrease Stock
                    product.StockQuantity -= item.Quantity;

                    _context.SaleItems.Add(item);
                }

                sale.TotalAmount = total;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                ModelState.AddModelError("", ex.Message);

                await LoadProducts();

                return View(sale);
            }
        }

        private async Task LoadProducts()
        {
            var products = await _context.Products
                .Where(p => p.StockQuantity > 0)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            ViewBag.Products = new SelectList(
                products,
                "ProductID",
                "ProductName");
        }
    }
}