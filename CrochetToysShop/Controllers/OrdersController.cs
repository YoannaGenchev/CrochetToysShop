using CrochetToysShop.Data;
using CrochetToysShop.Models.ViewModels.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrochetToysShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db;

        public OrdersController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await db.Orders
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

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkCompleted(int id)
        {
            var order = db.Orders.Find(id);
            if (order == null) return NotFound();

            order.Status = "Completed";
            db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
