using CrochetToysShop.Data;
using CrochetToysShop.Services.Interfaces;
using CrochetToysShop.Models.Entities;
using CrochetToysShop.Models.ViewModels.Orders;
using CrochetToysShop.Models.ViewModels.Toys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrochetToysShop.Controllers
{
    public class ToysController : Controller
    {
        private readonly IToyService toyService;

        public ToysController(IToyService toyService)
        {
            this.toyService = toyService;
        }


        public async Task<IActionResult> Index()
        {
            var toys = await toyService.GetAllAsync();
            return View(toys);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new ToyFormViewModel
            {
                Categories = db.Categories
                    .OrderBy(c => c.Name)
                    .Select(c => new CategoryDropdownViewModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ToyFormViewModel model)

        {
            if (!ModelState.IsValid)
            {
                model.Categories = db.Categories
                    .OrderBy(c => c.Name)
                    .Select(c => new CategoryDropdownViewModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToList();

                return View(model);
            }

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

            db.Toys.Add(toy);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Играчката е добавена успешно.";
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
        [Authorize(Roles = "Admin")]
        [Route("Toys/Edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            var toy = db.Toys.Find(id);
            if (toy == null)
            {
                return NotFound();
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
                Categories = db.Categories
                    .OrderBy(c => c.Name)
                    .Select(c => new CategoryDropdownViewModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [Route("Toys/Edit/{id:int}")]
        public IActionResult Edit(int id, ToyFormViewModel model)
        {
            var toy = db.Toys.Find(id);
            if (toy == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.Categories = db.Categories
                    .OrderBy(c => c.Name)
                    .Select(c => new CategoryDropdownViewModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToList();

                return View(model);
            }

            toy.Name = model.Name;
            toy.Description = model.Description;
            toy.Price = model.Price;
            toy.ImageUrl = model.ImageUrl;
            toy.SizeCm = model.SizeCm;
            toy.Difficulty = model.Difficulty;
            toy.IsAvailable = model.IsAvailable;
            toy.CategoryId = model.CategoryId;

            db.SaveChanges();

            TempData["SuccessMessage"] = "Промените са запазени успешно.";
            return RedirectToAction(nameof(Details), new { id = toy.Id });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("Toys/Delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var toy = db.Toys
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new ToyDeleteViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .FirstOrDefault();

            if (toy == null)
            {
                return NotFound();
            }

            return View(toy);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [Route("Toys/Delete/{id:int}")]
        public IActionResult DeleteConfirmed(int id)
        {
            var toy = db.Toys.Find(id);
            if (toy == null)
            {
                return NotFound();
            }

            db.Toys.Remove(toy);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Играчката е изтрита успешно.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("Toys/Order/{id:int}")]
        public IActionResult Order(int id)
        {
            var toy = db.Toys
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new { t.Id, t.Name, t.IsAvailable })
                .FirstOrDefault();

            if (toy == null) return NotFound();
            if (!toy.IsAvailable) return RedirectToAction(nameof(Details), new { id });

            return View(new OrderCreateViewModel
            {
                ToyId = toy.Id,
                ToyName = toy.Name
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Order(OrderCreateViewModel model)
             {
            var toy = db.Toys.Find(model.ToyId);
            if (toy == null) return NotFound();

            if (!toy.IsAvailable)
            {
                ModelState.AddModelError(string.Empty, "Тази играчка вече е изчерпана.");
            }

            if (!ModelState.IsValid)
            {
                model.ToyName = toy.Name;
                return View(model);
            }

            db.Orders.Add(new Order
            {
                CustomerName = model.CustomerName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                ToyId = toy.Id,
                Status = "New"
            });

            toy.IsAvailable = false;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Поръчката е изпратена успешно!";
            return RedirectToAction(nameof(Details), new { id = toy.Id });
        }

    }
}
