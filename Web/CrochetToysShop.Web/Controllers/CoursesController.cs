using CrochetToysShop.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static CrochetToysShop.Common.Constants.ApplicationConstants;

namespace CrochetToysShop.Web.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICourseService courseService;

        public CoursesController(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? difficulty)
        {
            var model = await courseService.GetAllAsync(difficulty);
            return View(model);
        }

        [HttpGet]
        [Route("Courses/Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var model = await courseService.GetDetailsAsync(id, userId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [Route("Courses/Enroll/{id:int}")]
        public async Task<IActionResult> Enroll(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await courseService.EnrollAsync(id, userId);

            if (!success)
            {
                TempData[TempDataKeys.ErrorMessage] = "Failed to enroll in course. It may be full or you are already enrolled.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData[TempDataKeys.SuccessMessage] = "Successfully enrolled in course!";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
