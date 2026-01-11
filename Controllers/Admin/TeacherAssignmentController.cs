using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class TeacherAssignmentController : Controller
    {
        private readonly AppDbContext _context;

        public TeacherAssignmentController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // List all active assignments (Classes)
            var classAssignments = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .OrderBy(c => c.StartDate)
                .ToListAsync();
            
            return View(classAssignments);
        }

        [HttpGet]
        public async Task<IActionResult> AssignToClass(string id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return NotFound();

            ViewBag.Instructors = await _context.Users.Where(u => u.Role.Name == "Instructor").ToListAsync();
            return View(@class);
        }

        [HttpPost]
        public async Task<IActionResult> AssignToClass(string id, string instructorId)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return NotFound();

            @class.InstructorId = instructorId;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Implementation for Exam assignments would require ExamAssignment model
    }
}
