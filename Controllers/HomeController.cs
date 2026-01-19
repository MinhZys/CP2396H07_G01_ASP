using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Symphony.Portal.Web.Models;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;

namespace Symphony.Portal.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.CourseReviews)
                .Where(c => c.IsActive)
                .ToListAsync();
            return View(courses);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.CourseSubjects)
                .ThenInclude(cs => cs.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();

            return View(course);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
