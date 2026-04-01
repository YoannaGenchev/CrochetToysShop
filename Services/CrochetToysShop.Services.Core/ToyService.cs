using CrochetToysShop.Data;
using CrochetToysShop.Data.Models;
using CrochetToysShop.Services.Core.Interfaces;
using CrochetToysShop.Web.ViewModels.Toys;
using Microsoft.EntityFrameworkCore;

namespace CrochetToysShop.Services.Core
{
    public class ToyService : IToyService
    {
        private readonly ApplicationDbContext db;

        public ToyService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<ToyIndexViewModel> GetAllAsync(int? categoryId = null)
        {
            var query = db.Toys
                .AsNoTracking()
                .Include(t => t.Category)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            var toys = await query
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

            var categories = await db.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDropdownViewModel {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return new ToyIndexViewModel
            {
                Toys = toys,
                CategoryId = categoryId,
                Categories = categories
            };
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
                    CategoryName = t.Category.Name,
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
                    .Select(c => new CategoryDropdownViewModel { Id = c.Id, Name = c.Name })
                    .ToListAsync(),
            };
        }

        public async Task CreateAsync(ToyFormViewModel model, string? userId = null)
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
                CategoryId = model.CategoryId,
                CreatedByUserId = userId
            };

            await db.Toys.AddAsync(toy);
            await db.SaveChangesAsync();
        }

        public async Task<ToyFormViewModel?> GetEditModelAsync(int id, string? userId = null)
        {
            var toy = await db.Toys
                .AsNoTracking()
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();

            if (toy == null)
            {
                return null;
            }

            // Check ownership
            if (!string.IsNullOrEmpty(userId) && toy.CreatedByUserId != userId)
            {
                return null; // Unauthorized - not the creator
            }

            var model = new ToyFormViewModel
            {
                Name = toy.Name,
                Description = toy.Description,
                Price = toy.Price,
                ImageUrl = toy.ImageUrl,
                SizeCm = toy.SizeCm,
                Difficulty = toy.Difficulty,
                IsAvailable = toy.IsAvailable,
                CategoryId = toy.CategoryId,
            };

            model.Categories = await db.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDropdownViewModel { Id = c.Id, Name = c.Name })
                .ToListAsync();

            return model;
        }

        public async Task<bool> EditAsync(int id, ToyFormViewModel model, string? userId = null)
        {
            var toy = await db.Toys.FindAsync(id);
            if (toy == null)
            {
                return false;
            }

            // Check ownership
            if (!string.IsNullOrEmpty(userId) && toy.CreatedByUserId != userId)
            {
                return false; // Unauthorized - not the creator
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

        public async Task<ToyDeleteViewModel?> GetDeleteModelAsync(int id, string? userId = null)
        {
            var toy = await db.Toys
                .AsNoTracking()
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();

            if (toy == null)
            {
                return null;
            }

            // Check ownership
            if (!string.IsNullOrEmpty(userId) && toy.CreatedByUserId != userId)
            {
                return null; // Unauthorized - not the creator
            }

            return new ToyDeleteViewModel { Id = toy.Id, Name = toy.Name };
        }

        public async Task<bool> DeleteAsync(int id, string? userId = null)
        {
            var toy = await db.Toys.FindAsync(id);
            if (toy == null)
            {
                return false;
            }

            // Check ownership
            if (!string.IsNullOrEmpty(userId) && toy.CreatedByUserId != userId)
            {
                return false; // Unauthorized - not the creator
            }

            db.Toys.Remove(toy);
            await db.SaveChangesAsync();
            return true;
        }
    }
}


