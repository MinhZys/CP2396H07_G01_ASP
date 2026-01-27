using Microsoft.AspNetCore.Mvc;

namespace CP2396H07_G01.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
