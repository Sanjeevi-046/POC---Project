using Microsoft.AspNetCore.Mvc;

namespace POC.MVC.Controllers
{
    public class ErrorHandlingController : Controller
    {
        [HttpGet]
        public IActionResult UnAuthorized(string statusCode)
        {
            ViewBag.StatusCode = statusCode;
            return View();
        }
    }
}
