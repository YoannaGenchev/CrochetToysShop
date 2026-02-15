using CrochetToysShop.Data;
using CrochetToysShop.Models.ViewModels.Toys;
using CrochetToysShop.Models.Entities;
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
        public async Task<ToyFormViewModel> GetCreateModelAsync()
        {
            return new ToyFormViewModel
            {
                Categories = await db.Categories
                    .AsNoTracking()
                    .OrderBy(c => c.Name)
                    .Select(c => new CategoryDropdownViewModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToListAsync()
            };
        }
        public async Task CreateAsync(ToyFormViewModel model)
        {
            var toy = new Toy
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                SizeCm = model.SizeCm,
                Difficulty = model.Difficulty,
                IsAvailable = model.IsAvailable,
                CategoryId = model.CategoryId
            };

            await db.Toys.AddAsync(toy);
            await db.SaveChangesAsync();
        }
        public async Task<ToyFormViewModel?> GetEditModelAsync(int id)
        {
            var toy = await db.Toys
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new ToyFormViewModel
                {
                    Name = t.Name,
                    Description = t.Description,
                    Price = t.Price,
                    ImageUrl = t.ImageUrl,
                    SizeCm = t.SizeCm,
                    Difficulty = t.Difficulty,
                    IsAvailable = t.IsAvailable,
                    CategoryId = t.CategoryId
                })
                .FirstOrDefaultAsync();

            if (toy == null)
            {
                return null;
            }

            toy.Categories = await db.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDropdownViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return toy;
        }
        public async Task<bool> EditAsync(int id, ToyFormViewModel model)
        {
            var toy = await db.Toys.FindAsync(id);
            if (toy == null)
            {
                return false;
            }

            toy.Name = model.Name;
            toy.Description = model.Description;
            toy.Price = model.Price;
            toy.ImageUrl = model.ImageUrl;
            toy.SizeCm = model.SizeCm;
            toy.Difficulty = model.Difficulty;
            toy.IsAvailable = model.IsAvailable;
            toy.CategoryId = model.CategoryId;

            await db.SaveChangesAsync();
            return true;
        }
        public async Task<ToyDeleteViewModel?> GetDeleteModelAsync(int id)
        {
            return await db.Toys
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new ToyDeleteViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .FirstOrDefaultAsync();
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var toy = await db.Toys.FindAsync(id);
            if (toy == null)
            {
                return false;
            }

            db.Toys.Remove(toy);
            await db.SaveChangesAsync();
            return true;
        }


    }
}
