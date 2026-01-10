using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Symphony.Portal.Web.Controllers.Student
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]/[action]")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
