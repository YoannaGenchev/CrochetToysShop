using CrochetToysShop.Services.Core.Interfaces;
using CrochetToysShop.Web.ViewModels.Orders;
using CrochetToysShop.Web.ViewModels.Toys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CrochetToysShop.Common.Constants.ApplicationConstants;

namespace CrochetToysShop.Web.Controllers
{
    public class ToysController : Controller
    {
        private readonly IToyService toyService;
        private readonly IOrderService orderService;

        public ToysController(IToyService toyService, IOrderService orderService)
        {
            this.toyService = toyService;
            this.orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var toys = await toyService.GetAllAsync();
            return View(toys);
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create()
        {
            var model = await toyService.GetCreateModelAsync();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ToyFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await toyService.GetCreateModelAsync();
                return View(model);
            }

            await toyService.CreateAsync(model);

            TempData[TempDataKeys.SuccessMessage] = SuccessMessages.ToyCreated;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("Toys/Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var toy = await toyService.GetDetailsAsync(id);

            if (toy == null)
            {
                return NotFound();
            }

            return View(toy);
        }

        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        [Route("Toys/Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await toyService.GetEditModelAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ValidateAntiForgeryToken]
        [Route("Toys/Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, ToyFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var formModel = await toyService.GetEditModelAsync(id);
                if (formModel == null)
                {
                    return NotFound();
                }

                model.Categories = formModel.Categories;
                return View(model);
            }

            var ok = await toyService.EditAsync(id, model);
            if (!ok)
            {
                return NotFound();
            }

            TempData[TempDataKeys.SuccessMessage] = SuccessMessages.ToyEdited;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        [Route("Toys/Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await toyService.GetDeleteModelAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ValidateAntiForgeryToken]
        [Route("Toys/Delete/{id:int}")]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ok = await toyService.DeleteAsync(id);
            if (!ok)
            {
                return NotFound();
            }

            TempData[TempDataKeys.SuccessMessage] = SuccessMessages.ToyDeleted;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("Toys/Order/{id:int}")]
        public async Task<IActionResult> Order(int id)
        {
            var model = await orderService.GetOrderModelAsync(id);

            if (model == null)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order(OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var orderModel = await orderService.GetOrderModelAsync(model.ToyId);
                model.ToyName = orderModel?.ToyName ?? model.ToyName;
                return View(model);
            }

            var (ok, error, toyId) = await orderService.CreateOrderAsync(model);

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error!);
                var orderModel = await orderService.GetOrderModelAsync(model.ToyId);
                model.ToyName = orderModel?.ToyName ?? model.ToyName;
                return View(model);
            }

            TempData[TempDataKeys.SuccessMessage] = SuccessMessages.OrderCreated;
            return RedirectToAction(nameof(Details), new { id = toyId });
        }
    }
}
