using CrochetToysShop.Services.Core.Interfaces;
using CrochetToysShop.Web.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index(string? difficulty, int page = 1)
        {
            var model = await courseService.GetAllAsync(difficulty, page);
            return View(model);
        }

        [HttpGet]
        [Route("Courses/Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.GetUserId();
            var model = await courseService.GetDetailsAsync(id, userId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = Roles.User)]
        [Route("Courses/MyCourses")]
        public async Task<IActionResult> MyCourses()
        {
            var userId = User.GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            if (User.IsInRole(Roles.Admin))
            {
                TempData[TempDataKeys.ErrorMessage] = "Admins cannot access My Courses.";
                return RedirectToAction(nameof(Index));
            }

            var model = await courseService.GetEnrolledCoursesAsync(userId);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Roles.User)]
        [ValidateAntiForgeryToken]
        [Route("Courses/Enroll/{id:int}")]
        public async Task<IActionResult> Enroll(int id)
        {
            var userId = User.GetUserId();
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (User.IsInRole(Roles.Admin))
            {
                TempData[TempDataKeys.ErrorMessage] = "Admins cannot enroll in courses.";
                return RedirectToAction(nameof(Details), new { id });
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
