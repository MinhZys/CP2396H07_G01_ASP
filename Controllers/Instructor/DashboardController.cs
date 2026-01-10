using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers.Instructor
{
    [Area("Instructor")]
    [Authorize(Roles = RoleNames.Instructor)]
    [Route("Instructor/[controller]/[action]")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Instructor Dashboard";
            return View();
        }
    }
}
