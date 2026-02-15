using CrochetToysShop.Data;
using CrochetToysShop.Models.ViewModels.Toys;
using CrochetToysShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrochetToysShop.Services.Implementations
{
    public class ToyService : IToyService
    {
        private readonly ApplicationDbContext db;

        public ToyService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IReadOnlyCollection<ToyListItemViewModel>> GetAllAsync()
        {
            return await db.Toys
                .AsNoTracking()
                .Include(t => t.Category)
                .OrderBy(t => t.Name)
                .Select(t => new ToyListItemViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Price = t.Price,
                    ImageUrl = t.ImageUrl,
                    CategoryName = t.Category.Name,
                    IsAvailable = t.IsAvailable
                })
                .ToListAsync();
        }

        public async Task<ToyDetailsViewModel?> GetDetailsAsync(int id)
        {
            return await db.Toys
                .AsNoTracking()
                .Include(t => t.Category)
                .Where(t => t.Id == id)
                .Select(t => new ToyDetailsViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Price = t.Price,
                    ImageUrl = t.ImageUrl,
                    SizeCm = t.SizeCm,
                    Difficulty = t.Difficulty,
                    IsAvailable = t.IsAvailable,
                    CategoryName = t.Category.Name
                })
                .FirstOrDefaultAsync();
        }
    }
}
