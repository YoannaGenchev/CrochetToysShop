using CrochetToysShop.Data;
using CrochetToysShop.Models.Entities;
using CrochetToysShop.Common;
using CrochetToysShop.Models.ViewModels.Orders;
using CrochetToysShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrochetToysShop.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext db;

        public OrderService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<OrderAdminListItemViewModel>> GetAllForAdminAsync()
        {
            return await db.Orders
                .AsNoTracking()
                .Include(o => o.Toy)
                .OrderByDescending(o => o.CreatedOn)
                .Select(o => new OrderAdminListItemViewModel
                {
                    Id = o.Id,
                    CreatedOn = o.CreatedOn,
                    Status = o.Status,
                    CustomerName = o.CustomerName,
                    PhoneNumber = o.PhoneNumber,
                    Address = o.Address,
                    ToyId = o.ToyId,
                    ToyName = o.Toy.Name
                })
                .ToListAsync();
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

            return new OrderCreateViewModel
            {
                ToyId = toy.Id,
                ToyName = toy.Name
            };
        }

        public async Task<(bool ok, string? error, int toyId)> CreateOrderAsync(OrderCreateViewModel model)
        {
            var toy = await db.Toys.FindAsync(model.ToyId);
            if (toy == null)
            {
                return (false, "Играчката не е намерена.", model.ToyId);
            }

            if (!toy.IsAvailable)
            {
                return (false, "Тази играчка вече е изчерпана.", toy.Id);
            }

            await db.Orders.AddAsync(new Order
            {
                CustomerName = model.CustomerName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                ToyId = toy.Id,
                Status = OrderStatus.New
            });

            toy.IsAvailable = false;
            await db.SaveChangesAsync();

            return (true, null, toy.Id);
        }
    }
}
