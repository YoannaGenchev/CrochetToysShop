using CrochetToysShop.Services.Core.Interfaces;
using CrochetToysShop.Web.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;

namespace CrochetToysShop.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categoryStats = await categoryService.GetAllAsync();
            return View(categoryStats);
        }

        [HttpGet]
        [Route("Categories/{categoryName}")]
        public async Task<IActionResult> Details(string categoryName)
        {
            var categoryToys = await categoryService.GetByNameAsync(categoryName);

            if (!categoryToys.Any())
            {
                return NotFound();
            }

            return RedirectToAction("Category", "Toys", new { categoryName });
        }
    }
}
