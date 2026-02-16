using CrochetToysShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrochetToysShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await orderService.GetAllForAdminAsync();
            return View(orders);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkCompleted(int id)
        {
            var ok = await orderService.MarkCompletedAsync(id);
            if (!ok) return NotFound();

            TempData["SuccessMessage"] = "Поръчката е маркирана като изпълнена.";
            return RedirectToAction(nameof(Index));
        }
    }
}
