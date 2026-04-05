using Microsoft.AspNetCore.Mvc;

namespace CrochetToysShop.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            Response.StatusCode = statusCode;

            return statusCode switch
            {
                404 => View("NotFound"),
                500 => View("ServerError"),
                _ => View("Error")
            };
        }

        [Route("/error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
