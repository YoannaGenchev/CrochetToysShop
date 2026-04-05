using CrochetToysShop.Data;
using CrochetToysShop.Services.Core.Interfaces;
using CrochetToysShop.Web.ViewModels.Categories;
using CrochetToysShop.Web.ViewModels.Toys;
using Microsoft.EntityFrameworkCore;

namespace CrochetToysShop.Services.Core
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext db;

        public CategoryService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<CategoryListItemViewModel>> GetAllAsync()
        {
            return await db.Toys
                .AsNoTracking()
                .Include(t => t.Category)
                .GroupBy(t => t.Category.Name)
                .Select(g => new CategoryListItemViewModel
                {
                    Name = g.Key,
                    ToyCount = g.Count()
                })
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<ToyListItemViewModel>> GetByNameAsync(string categoryName)
        {
            var normalizedCategoryName = categoryName.Trim().ToLower();

            return await db.Toys
                .AsNoTracking()
                .Include(t => t.Category)
                .Where(t => t.Category.Name.ToLower() == normalizedCategoryName)
                .OrderBy(t => t.Name)
                .Select(t => new ToyListItemViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Price = t.Price,
                    ImageUrl = t.ImageUrl,
                    CategoryName = t.Category.Name,
                    IsAvailable = t.IsAvailable,
                })
                .ToListAsync();
        }
    }
}
