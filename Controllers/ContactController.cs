using Microsoft.AspNetCore.Mvc;

namespace Symphony.Portal.Web.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();
        }
    }
}
