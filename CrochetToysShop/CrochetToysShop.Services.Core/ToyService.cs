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

        public async Task<ToyIndexViewModel> GetAllAsync(int? categoryId = null, int page = 1, int pageSize = 10)
        {
            var query = db.Toys
                .AsNoTracking()
                .Include(t => t.Category)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Ensure page is within valid range
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var toys = await query
                .OrderBy(t => t.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
                Categories = categories,
                Pagination = new Web.ViewModels.Common.PaginationViewModel
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                }
            };
        }

        public async Task<ToyIndexViewModel> SearchAsync(string? searchTerm = null, int? categoryId = null, int page = 1, int pageSize = 10)
        {
            var query = db.Toys
                .AsNoTracking()
                .Include(t => t.Category)
                .AsQueryable();

            // Filter by search term (name or description)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(t => t.Name.ToLower().Contains(lowerSearchTerm) || 
                                         t.Description.ToLower().Contains(lowerSearchTerm));
            }

            // Filter by category
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Ensure page is within valid range
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var toys = await query
                .OrderBy(t => t.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
                Categories = categories,
                Pagination = new Web.ViewModels.Common.PaginationViewModel
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                }
            };
        }

        public async Task<ToyIndexViewModel> GetByCategoryNameAsync(string categoryName)
        {
            var normalizedCategoryName = categoryName.Trim().ToLower();

            var filteredToys = await db.Toys
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

            var categories = await db.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDropdownViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return new ToyIndexViewModel
            {
                Toys = filteredToys,
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


