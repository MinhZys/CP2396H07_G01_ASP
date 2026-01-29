using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;
using System.Security.Claims;

namespace Symphony.Portal.Web.Controllers
{
    public class ExamResultController : Controller
    {
        private readonly AppDbContext _context;

        public ExamResultController(AppDbContext context)
        {
            _context = context;
        }

        // List of all exams for the current student
        public async Task<IActionResult> MyExams()
        {
            // Assuming we use standard ASP.NET Identity or a custom UserId claim
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var sessions = await _context.StudentExamSessions
                .Include(s => s.EntranceExam)
                .Include(s => s.ExamPaper)
                .Where(s => s.StudentId == userId)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();

            return View(sessions);
        }

        // Detailed result of a session
        public async Task<IActionResult> Details(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var session = await _context.StudentExamSessions
                .Include(s => s.EntranceExam)
                .Include(s => s.ExamPaper)
                .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
                .ThenInclude(q => q!.Options)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();

            // Security check: only the student who took the exam or an admin can view results
            // if (session.StudentId != userId && !User.IsInRole("Admin")) return Forbid();

            return View(session);
        }
    }
}
