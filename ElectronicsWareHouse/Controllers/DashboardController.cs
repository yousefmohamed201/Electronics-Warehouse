using ElectronicsWareHouse.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsWareHouse.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // =========================
            // KPI COUNTS
            // =========================

            ViewBag.TotalProducts = await _context.Products.CountAsync();

            ViewBag.TotalCategories = await _context.Categories.CountAsync();

            ViewBag.TotalSuppliers = await _context.Suppliers.CountAsync();

            ViewBag.TotalSales = await _context.Sales.CountAsync();

            ViewBag.TotalPurchases = await _context.Purchases.CountAsync();


            // =========================
            // STOCK
            // =========================

            ViewBag.TotalStockQuantity = await _context.Products
                .SumAsync(p => (int?)p.StockQuantity) ?? 0;

            ViewBag.LowStockCount = await _context.Products
                .CountAsync(p => p.StockQuantity <= p.LowStockThreshold);


            // =========================
            // SALES / PURCHASES VALUES
            // =========================

            ViewBag.SalesAmount = await _context.Sales
                .SumAsync(s => (decimal?)s.TotalAmount) ?? 0;

            ViewBag.PurchasesAmount = await _context.Purchases
                .SumAsync(p => (decimal?)p.TotalAmount) ?? 0;


            // =========================
            // RECENT SALES
            // =========================

            var recentSales = await _context.Sales
                .OrderByDescending(s => s.SaleDate)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentSales = recentSales;


            // =========================
            // RECENT PURCHASES
            // =========================

            var recentPurchases = await _context.Purchases
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.PurchaseDate)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentPurchases = recentPurchases;


            // =========================
            // MOST SOLD PRODUCTS
            // =========================

            var mostSoldProducts = await _context.SaleItems
                .Include(si => si.Product)
                .GroupBy(si => new
                {
                    si.ProductID,
                    ProductName = si.Product!.ProductName,
                    SKU = si.Product.SKU
                })
                .Select(g => new
                {
                    ProductName = g.Key.ProductName,
                    SKU = g.Key.SKU,
                    QuantitySold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(5)
                .ToListAsync();

            ViewBag.MostSoldProducts = mostSoldProducts;


            // =========================
            // CHART DATA
            // =========================

            var salesChart = await _context.Sales
                .GroupBy(s => s.SaleDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Amount = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .Take(30)
                .ToListAsync();

            ViewBag.SalesChartLabels = salesChart
                .Select(x => x.Date.ToString("MM/dd"))
                .ToList();

            ViewBag.SalesChartData = salesChart
                .Select(x => x.Amount)
                .ToList();


            var purchaseChart = await _context.Purchases
                .GroupBy(p => p.PurchaseDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Amount = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .Take(30)
                .ToListAsync();

            ViewBag.PurchaseChartLabels = purchaseChart
                .Select(x => x.Date.ToString("MM/dd"))
                .ToList();

            ViewBag.PurchaseChartData = purchaseChart
                .Select(x => x.Amount)
                .ToList();


            return View();
        }
    }
}