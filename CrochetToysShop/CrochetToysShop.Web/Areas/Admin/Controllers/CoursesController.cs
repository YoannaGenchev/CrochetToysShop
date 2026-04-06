using CrochetToysShop.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CrochetToysShop.Common.Constants.ApplicationConstants;

namespace CrochetToysShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class CoursesController : Controller
    {
        private readonly ICourseService courseService;

        public CoursesController(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> Enrollments()
        {
            var model = await courseService.GetAllEnrollmentsForAdminAsync();
            return View(model);
        }
    }
}
