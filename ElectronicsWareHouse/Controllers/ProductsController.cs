using ElectronicsWareHouse.Data;
using ElectronicsWareHouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsWareHouse.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.PurchaseItems)
                .Include(p => p.SaleItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            await LoadCategories();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!string.IsNullOrWhiteSpace(product.SKU) &&
                await _context.Products.AnyAsync(p => p.SKU.ToLower() == product.SKU.Trim().ToLower()))
            {
                ModelState.AddModelError("SKU", "A product with this SKU already exists.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategories(product.CategoryID);
                return View(product);
            }

            product.SKU = product.SKU.Trim().ToUpper();
            product.ProductName = product.ProductName.Trim();
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            await LoadCategories(product.CategoryID);

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductID)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(product.SKU) &&
                await _context.Products.AnyAsync(p => p.ProductID != id && p.SKU.ToLower() == product.SKU.Trim().ToLower()))
            {
                ModelState.AddModelError("SKU", "Another product with this SKU already exists.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategories(product.CategoryID);
                return View(product);
            }

            try
            {
                product.SKU = product.SKU.Trim().ToUpper();
                product.ProductName = product.ProductName.Trim();
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.ProductID == product.ProductID))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.PurchaseItems)
                .Include(p => p.SaleItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .Include(p => p.PurchaseItems)
                .Include(p => p.SaleItems)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                return NotFound();

            if (product.PurchaseItems.Any() || product.SaleItems.Any())
            {
                TempData["ErrorMessage"] = $"Cannot delete product '{product.ProductName}' because it is linked to existing transactions (Purchases/Sales).";
                return RedirectToAction(nameof(Index));
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategories(int? selectedCategory = null)
        {
            var categories = await _context.Categories
                .OrderBy(c => c.CategoryName)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.CategoryID = new SelectList(
                categories,
                "CategoryID",
                "CategoryName",
                selectedCategory);
        }
    }
}