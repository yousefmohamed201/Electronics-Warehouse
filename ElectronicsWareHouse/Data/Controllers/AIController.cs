using ElectronicsWareHouse.Data;
using ElectronicsWareHouse.Services;
using ElectronicsWareHouse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ElectronicsWareHouse.Controllers
{
    public class AIController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GeminiAIService _geminiAIService;

        public AIController(
            ApplicationDbContext context,
            GeminiAIService geminiAIService)
        {
            _context = context;
            _geminiAIService = geminiAIService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new AIViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AIViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Question))
            {
                model.Error = "Please enter a question.";
                return View(model);
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Select(p => new AIProductViewModel
                {
                    ProductName = p.ProductName,
                    SKU = p.SKU,
                    Category = p.Category != null
                        ? p.Category.CategoryName
                        : "No Category",
                    UnitPrice = p.UnitPrice,
                    StockQuantity = p.StockQuantity,
                    LowStockThreshold = p.LowStockThreshold
                })
                .ToListAsync();

            var lowStockProducts = products
                .Where(p => p.StockQuantity <= p.LowStockThreshold)
                .Select(p => new AILowStockProductViewModel
                {
                    ProductName = p.ProductName,
                    SKU = p.SKU,
                    StockQuantity = p.StockQuantity,
                    LowStockThreshold = p.LowStockThreshold
                })
                .ToList();

            var sales = await _context.SaleItems
                .Include(si => si.Product)
                .Where(si => si.Product != null)
                .GroupBy(si => new
                {
                    si.ProductID,
                    ProductName = si.Product!.ProductName
                })
                .Select(g => new AISalesSummaryViewModel
                {
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.QuantitySold)
                .ToListAsync();

            var inventoryData = new AIInventoryDataViewModel
            {
                Products = products,
                LowStockProducts = lowStockProducts,
                SalesSummary = sales
            };

            var inventoryJson = JsonSerializer.Serialize(
                inventoryData,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            var prompt = $"""
You are an AI Inventory Assistant for an Electronics Warehouse Management System.

You must answer the user's question using only the inventory data provided below.

Important rules:
- Use only the provided inventory data for factual answers.
- Do not invent products, quantities, prices, sales, or stock levels.
- If the data is not enough to answer the question, clearly say that.
- Give practical and concise recommendations when appropriate.
- If the user asks about low stock, identify products whose StockQuantity is less than or equal to LowStockThreshold.
- If the user asks about best-selling products, use QuantitySold and Revenue.
- Currency is EGP.
- Answer in a clear and professional way.

USER QUESTION:
{model.Question}

INVENTORY DATA:
{inventoryJson}
""";

            try
            {
                model.Answer =
                    await _geminiAIService.AskGeminiAsync(prompt);
            }
            catch (Exception ex)
            {
                model.Error =
                    $"An error occurred while contacting Gemini: {ex.Message}";
            }

            return View(model);
        }
    }
}