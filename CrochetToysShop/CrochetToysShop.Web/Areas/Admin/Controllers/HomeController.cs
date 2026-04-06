using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CrochetToysShop.Common.Constants.ApplicationConstants;

namespace CrochetToysShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
