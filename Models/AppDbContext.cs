using Microsoft.AspNetCore.Mvc;

namespace CP2396H07_G01.Models
{
    public class AppDbContext : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
