using CrochetToysShop.Services.Core.Interfaces;
using CrochetToysShop.Web.ViewModels.Categories;
using CrochetToysShop.Web.ViewModels.Toys;

namespace CrochetToysShop.Services.Core
{
    public class CategoryService : ICategoryService
    {
        private readonly IToyService toyService;

        public CategoryService(IToyService toyService)
        {
            this.toyService = toyService;
        }

        public async Task<IEnumerable<CategoryListItemViewModel>> GetAllAsync()
        {
            var toysModel = await toyService.GetAllAsync();

            return toysModel.Toys
                .GroupBy(t => t.CategoryName)
                .Select(g => new CategoryListItemViewModel
                {
                    Name = g.Key,
                    ToyCount = g.Count()
                })
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<IEnumerable<ToyListItemViewModel>> GetByNameAsync(string categoryName)
        {
            var toysModel = await toyService.GetAllAsync();

            return toysModel.Toys
                .Where(t => t.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
