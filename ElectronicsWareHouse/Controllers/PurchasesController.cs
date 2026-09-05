using ElectronicsWareHouse.Data;
using ElectronicsWareHouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsWareHouse.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Purchases
        public async Task<IActionResult> Index(
            string? supplierSearch,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1)
        {
            const int pageSize = 10;

            var query = _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(supplierSearch))
            {
                query = query.Where(p =>
                    p.Supplier.SupplierName.Contains(supplierSearch));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(p => p.PurchaseDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                // Include the whole "to" day
                var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.PurchaseDate <= endOfDay);
            }

            query = query.OrderByDescending(p => p.PurchaseDate);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;

            var purchases = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SupplierSearch = supplierSearch;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(purchases);
        }

        // GET: Purchases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(p => p.PurchaseID == id);

            if (purchase == null)
                return NotFound();

            return View(purchase);
        }

        // GET: Purchases/Create
        public async Task<IActionResult> Create()
        {
            await LoadSuppliers();
            await LoadProducts();

            return View();
        }

        // POST: Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Purchase purchase,
            List<PurchaseItem>? items)
        {
            if (items == null || !items.Any())
            {
                ModelState.AddModelError("", "Purchase must contain at least one product.");

                await LoadSuppliers(purchase.SupplierID);
                await LoadProducts();

                return View(purchase);
            }

            if (!ModelState.IsValid)
            {
                await LoadSuppliers(purchase.SupplierID);
                await LoadProducts();

                return View(purchase);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                purchase.PurchaseDate = DateTime.Now;
                purchase.TotalAmount = 0;

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();

                decimal total = 0;

                foreach (var item in items)
                {
                    if (item.Quantity <= 0)
                        throw new Exception("Quantity must be greater than zero.");

                    if (item.UnitCost < 0)
                        throw new Exception("Unit cost cannot be negative.");

                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.ProductID == item.ProductID);

                    if (product == null)
                        throw new Exception("Product not found.");

                    item.PurchaseID = purchase.PurchaseID;

                    total += item.Quantity * item.UnitCost;

                    // Increase Stock
                    product.StockQuantity += item.Quantity;

                    _context.PurchaseItems.Add(item);
                }

                purchase.TotalAmount = total;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();

                ModelState.AddModelError(
                    "",
                    "An error occurred while creating the purchase.");

                await LoadSuppliers(purchase.SupplierID);
                await LoadProducts();

                return View(purchase);
            }
        }

        // GET: Purchases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.PurchaseID == id);

            if (purchase == null)
                return NotFound();

            return View(purchase);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.PurchaseID == id);

            if (purchase == null)
                return NotFound();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in purchase.PurchaseItems)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.ProductID == item.ProductID);

                    if (product != null)
                    {
                        product.StockQuantity -= item.Quantity;

                        if (product.StockQuantity < 0)
                            throw new Exception(
                                "Cannot delete purchase because stock would become negative.");
                    }
                }

                _context.Purchases.Remove(purchase);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();

                TempData["Error"] =
                    "Cannot delete this purchase because it would make stock negative.";

                return RedirectToAction(nameof(Index));
            }
        }

        private async Task LoadSuppliers(int? selectedSupplier = null)
        {
            var suppliers = await _context.Suppliers
                .OrderBy(s => s.SupplierName)
                .ToListAsync();

            ViewBag.SupplierID = new SelectList(
                suppliers,
                "SupplierID",
                "SupplierName",
                selectedSupplier);
        }

        private async Task LoadProducts()
        {
            var products = await _context.Products
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            ViewBag.Products = new SelectList(
                products,
                "ProductID",
                "ProductName");
        }
    }
}