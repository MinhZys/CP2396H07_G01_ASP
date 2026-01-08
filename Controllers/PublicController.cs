using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models.ViewModels;

namespace Symphony.Portal.Web.Controllers
{
    public class PublicController : Controller
    {
        private readonly AppDbContext _context;

        public PublicController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                FeaturedCourses = await _context.Courses.Where(c => c.IsActive).ToListAsync(),
                UpcomingExams = await _context.AdmissionExams.Where(e => e.IsActive && e.ExamDate > DateTime.Now).OrderBy(e => e.ExamDate).Take(3).ToListAsync()
            };
            return View(viewModel);
        }
    }
}
