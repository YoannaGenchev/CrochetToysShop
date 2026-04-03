using CrochetToysShop.Services.Core.Interfaces;
using CrochetToysShop.Web.Infrastructure.Extensions;
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

        public async Task<IActionResult> Index(int? categoryId, int page = 1)
        {
            var model = await toyService.GetAllAsync(categoryId, page);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? q, int? categoryId, int page = 1)
        {
            var model = await toyService.SearchAsync(q, categoryId, page);
            ViewBag.SearchQuery = q;
            return View("Index", model);
        }

        [HttpGet]
        [Route("Toys/Category/{categoryName}")]
        public async Task<IActionResult> Category(string categoryName)
        {
            var categoryModel = await toyService.GetByCategoryNameAsync(categoryName);

            if (!categoryModel.Toys.Any())
            {
                ViewBag.CategoryName = categoryName;
            }

            return View("Index", categoryModel);
        }
        [HttpGet]
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
                var categoriesModel = await toyService.GetCreateModelAsync();
                model.Categories = categoriesModel.Categories;
                return View(model);
            }

            var userId = User.GetUserId();
            await toyService.CreateAsync(model, userId);

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
            var userId = User.GetUserId();
            var model = await toyService.GetEditModelAsync(id, userId);
            if (model == null)
            {
                return Forbid();
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ValidateAntiForgeryToken]
        [Route("Toys/Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, ToyFormViewModel model)
        {
            var userId = User.GetUserId();

            if (!ModelState.IsValid)
            {
                var formModel = await toyService.GetEditModelAsync(id, userId);
                if (formModel == null)
                {
                    return Forbid();
                }

                model.Categories = formModel.Categories;
                return View(model);
            }

            var ok = await toyService.EditAsync(id, model, userId);
            if (!ok)
            {
                return Forbid();
            }

            TempData[TempDataKeys.SuccessMessage] = SuccessMessages.ToyEdited;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        [Route("Toys/Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.GetUserId();
            var model = await toyService.GetDeleteModelAsync(id, userId);
            if (model == null)
            {
                return Forbid();
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
            var userId = User.GetUserId();
            var ok = await toyService.DeleteAsync(id, userId);
            if (!ok)
            {
                return Forbid();
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
