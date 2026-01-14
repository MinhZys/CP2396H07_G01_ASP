using Microsoft.AspNetCore.Mvc;

namespace Symphony.Portal.Web.Controllers
{
    public class MentorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
