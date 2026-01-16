using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;
using System.Security.Claims;

namespace Symphony.Portal.Web.Controllers.Instructor
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    [Route("Instructor/[controller]/[action]")]
    public class AssignmentsController : Controller
    {
        private readonly AppDbContext _context;

        public AssignmentsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var assignments = await _context.Assignments
                .Include(a => a.Class)
                .ThenInclude(c => c.ClassCategory)
                .Where(a => a.InstructorId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(assignments);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var assignment = await _context.Assignments
                .Include(a => a.Class)
                .ThenInclude(c => c.ClassCategory)
                .FirstOrDefaultAsync(a => a.Id == id && a.InstructorId == userId);

            if (assignment == null) return NotFound();

            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(string id, AssignmentStatus status, string? cancellationReason)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == id && a.InstructorId == userId);
            
            if (assignment == null) return NotFound();

            assignment.Status = status;
            if (status == AssignmentStatus.Cancelled && !string.IsNullOrEmpty(cancellationReason))
            {
                assignment.CancellationReason = cancellationReason;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
