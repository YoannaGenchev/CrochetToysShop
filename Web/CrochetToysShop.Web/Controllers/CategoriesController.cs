using CrochetToysShop.Services.Core.Interfaces;
using CrochetToysShop.Web.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;

namespace CrochetToysShop.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IToyService toyService;

        public CategoriesController(IToyService toyService)
        {
            this.toyService = toyService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get all toys to count by category
            var toysModel = await toyService.GetAllAsync();
            
            var categoryStats = toysModel.Toys
                .GroupBy(t => t.CategoryName)
                .Select(g => new CategoryListItemViewModel
                {
                    Name = g.Key,
                    ToyCount = g.Count()
                })
                .OrderBy(c => c.Name)
                .ToList();

            return View(categoryStats);
        }

        [HttpGet]
        [Route("Categories/{categoryName}")]
        public async Task<IActionResult> Details(string categoryName)
        {
            // Get toys by category
            var toysModel = await toyService.GetAllAsync();
            var categoryToys = toysModel.Toys
                .Where(t => t.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!categoryToys.Any())
            {
                return NotFound();
            }

            ViewBag.CategoryName = categoryName;
            return View("Index", categoryToys);
        }
    }
}
