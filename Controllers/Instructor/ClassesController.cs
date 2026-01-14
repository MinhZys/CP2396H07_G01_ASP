using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using System.Security.Claims;

namespace Symphony.Portal.Web.Controllers.Instructor
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    [Route("Instructor/[controller]/[action]")]
    public class ClassesController : Controller
    {
        private readonly AppDbContext _context;

        public ClassesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var classes = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Enrollments)
                .Where(c => c.InstructorId == userId)
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();

            return View(classes);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Ensure the class belongs to this instructor
            var classEntity = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id && c.InstructorId == userId);

            if (classEntity == null) return NotFound();

            return View(classEntity);
        }
    }
}
