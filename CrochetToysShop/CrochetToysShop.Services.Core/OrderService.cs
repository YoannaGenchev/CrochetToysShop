using CrochetToysShop.Common;
using CrochetToysShop.Common.Constants;
using CrochetToysShop.Data;
using CrochetToysShop.Data.Models;
using CrochetToysShop.Services.Core.Interfaces;
using CrochetToysShop.Web.ViewModels.Orders;
using Microsoft.EntityFrameworkCore;

namespace CrochetToysShop.Services.Core
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext db;

        public OrderService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<OrderIndexViewModel> GetAllForAdminAsync(int page = 1, int pageSize = 10)
        {
            var query = db.Orders
                .AsNoTracking()
                .Include(o => o.Toy)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var orders = await query
                .OrderByDescending(o => o.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderAdminListItemViewModel
                {
                    Id = o.Id,
                    CreatedOn = o.CreatedOn,
                    Status = o.Status,
                    CustomerName = o.CustomerName,
                    PhoneNumber = o.PhoneNumber,
                    Address = o.Address,
                    ToyId = o.ToyId,
                    ToyName = o.Toy.Name,
                })
                .ToListAsync();

            return new OrderIndexViewModel
            {
                Orders = orders,
                Pagination = new Web.ViewModels.Common.PaginationViewModel
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                }
            };
        }

        public async Task<bool> MarkCompletedAsync(int id)
        {
            var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return false;
            }

            order.Status = OrderStatus.Completed;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<OrderCreateViewModel?> GetOrderModelAsync(int toyId)
        {
            var toy = await db.Toys
                .AsNoTracking()
                .Where(t => t.Id == toyId)
                .Select(t => new { t.Id, t.Name, t.IsAvailable })
                .FirstOrDefaultAsync();

            if (toy == null || !toy.IsAvailable)
            {
                return null;
            }

            return new OrderCreateViewModel { ToyId = toy.Id, ToyName = toy.Name };
        }

        public async Task<(bool ok, string? error, int toyId)> CreateOrderAsync(OrderCreateViewModel model)
        {
            var toy = await db.Toys.FindAsync(model.ToyId);
            if (toy == null)
            {
                return (false, ApplicationConstants.ErrorMessages.ToyNotFound, model.ToyId);
            }

            if (!toy.IsAvailable)
            {
                return (false, ApplicationConstants.ErrorMessages.ToyNotAvailable, toy.Id);
            }

            await db.Orders.AddAsync(new Order
            {
                CustomerName = model.CustomerName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                ToyId = toy.Id,
                Status = OrderStatus.New,
            });

            toy.IsAvailable = false;
            await db.SaveChangesAsync();

            return (true, null, toy.Id);
        }
    }
}
