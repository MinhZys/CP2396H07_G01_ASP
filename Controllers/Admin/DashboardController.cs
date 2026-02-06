using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models.ViewModels;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardVM
            {
                StudentCount = await _context.Users.CountAsync(u => u.Role.Name == "Student"),
                InstructorCount = await _context.Users.CountAsync(u => u.Role.Name == "Instructor"),
                CourseCount = await _context.Courses.CountAsync(c => c.IsActive),
                UpcomingExamCount = await _context.EntranceExams.CountAsync(e => e.ExamDate > DateTime.Now),
                PendingRegistrationCount = await _context.StudentRegistrations.CountAsync(r => r.Status == Symphony.Portal.Web.Models.Enums.RegistrationStatus.Pending)
            };

            return View(viewModel);
        }
    }
}
