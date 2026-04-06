using CrochetToysShop.Services.Core.Interfaces;
using CrochetToysShop.Web.Infrastructure.Extensions;
using CrochetToysShop.Web.ViewModels.Toys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CrochetToysShop.Common.Constants.ApplicationConstants;

namespace CrochetToysShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class ToysController : Controller
    {
        private readonly IToyService toyService;

        public ToysController(IToyService toyService)
        {
            this.toyService = toyService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await toyService.GetCreateModelAsync();
            return View(model);
        }

        [HttpPost]
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
            return RedirectToAction("Index", "Toys", new { area = "" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.GetUserId();
            var model = await toyService.GetEditModelAsync(id, userId, isAdmin: true);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ToyFormViewModel model)
        {
            var userId = User.GetUserId();

            if (!ModelState.IsValid)
            {
                var formModel = await toyService.GetEditModelAsync(id, userId, isAdmin: true);
                if (formModel == null)
                {
                    return NotFound();
                }

                model.Categories = formModel.Categories;
                return View(model);
            }

            var ok = await toyService.EditAsync(id, model, userId, isAdmin: true);
            if (!ok)
            {
                return NotFound();
            }

            TempData[TempDataKeys.SuccessMessage] = SuccessMessages.ToyEdited;
            return RedirectToAction("Details", "Toys", new { area = "", id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.GetUserId();
            var model = await toyService.GetDeleteModelAsync(id, userId, isAdmin: true);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.GetUserId();
            var ok = await toyService.DeleteAsync(id, userId, isAdmin: true);
            if (!ok)
            {
                return NotFound();
            }

            TempData[TempDataKeys.SuccessMessage] = SuccessMessages.ToyDeleted;
            return RedirectToAction("Index", "Toys", new { area = "" });
        }
    }
}
