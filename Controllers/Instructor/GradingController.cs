using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers.Instructor
{
    [Area("Instructor")]
    [Authorize(Roles = RoleNames.Instructor)]
    [Route("Instructor/[controller]/[action]")]
    public class GradingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Input(string id)
        {
            ViewBag.ExamId = id;
            return View();
        }
    }
}
