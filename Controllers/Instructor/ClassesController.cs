using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Symphony.Portal.Web.Controllers.Instructor
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    [Route("Instructor/[controller]/[action]")]
    public class ClassesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            ViewBag.ClassId = id;
            return View();
        }
    }
}
