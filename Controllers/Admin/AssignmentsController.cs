using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class AssignmentsController : Controller
    {
        private readonly AppDbContext _context;

        public AssignmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Assignments
        public async Task<IActionResult> Index(string? instructorId, string? classId, string? searchTerm)
        {
            var query = _context.Assignments
                .Include(a => a.Class)
                .ThenInclude(c => c.ClassCategory)
                .Include(a => a.Instructor)
                .AsQueryable();

            if (!string.IsNullOrEmpty(instructorId))
            {
                query = query.Where(a => a.InstructorId == instructorId);
            }

            if (!string.IsNullOrEmpty(classId))
            {
                query = query.Where(a => a.ClassId == classId);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Title.Contains(searchTerm) || a.TermOrExamName.Contains(searchTerm));
            }

            ViewBag.Instructors = new SelectList(_context.Users.Where(u => u.Role.Name == "Instructor"), "Id", "FullName");
            ViewBag.Classes = new SelectList(_context.Classes, "Id", "ClassName");

            return View(await query.OrderByDescending(a => a.CreatedAt).ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.Instructors = new SelectList(_context.Users.Where(u => u.Role.Name == "Instructor"), "Id", "FullName");
            ViewBag.Classes = new SelectList(_context.Classes.Where(c => c.Status == ClassStatus.Active), "Id", "ClassName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(assignment.Id)) assignment.Id = Guid.NewGuid().ToString();
                
                // Set default status if not set
                if (assignment.Status == 0) assignment.Status = AssignmentStatus.Assigned;
                assignment.CreatedAt = DateTime.Now;

                _context.Add(assignment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Assignment created successfully.";
                return RedirectToAction(nameof(Index));
            }
             ViewBag.Instructors = new SelectList(_context.Users.Where(u => u.Role.Name == "Instructor"), "Id", "FullName", assignment.InstructorId);
            ViewBag.Classes = new SelectList(_context.Classes.Where(c => c.Status == ClassStatus.Active), "Id", "ClassName", assignment.ClassId);
            return View(assignment);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null) return NotFound();
            
            ViewBag.Instructors = new SelectList(_context.Users.Where(u => u.Role.Name == "Instructor"), "Id", "FullName", assignment.InstructorId);
            ViewBag.Classes = new SelectList(_context.Classes.Where(c => c.Status == ClassStatus.Active), "Id", "ClassName", assignment.ClassId);
            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Assignment assignment)
        {
            if (id != assignment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                 try
                {
                    _context.Update(assignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Assignments.Any(e => e.Id == assignment.Id)) return NotFound();
                    else throw;
                }
                TempData["Success"] = "Assignment updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Instructors = new SelectList(_context.Users.Where(u => u.Role.Name == "Instructor"), "Id", "FullName", assignment.InstructorId);
            ViewBag.Classes = new SelectList(_context.Classes.Where(c => c.Status == ClassStatus.Active), "Id", "ClassName", assignment.ClassId);
            return View(assignment);
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return NotFound();

            var assignment = await _context.Assignments
                .Include(a => a.Class)
                .ThenInclude(c => c.ClassCategory)
                .Include(a => a.Instructor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignment == null)  return NotFound();

            return View(assignment);
        }

        public async Task<IActionResult> Delete(string? id)
        {
             if (id == null) return NotFound();
            var assignment = await _context.Assignments
                .Include(a => a.Class)
                .Include(a => a.Instructor)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (assignment == null) return NotFound();

            return View(assignment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment != null)
            {
                _context.Assignments.Remove(assignment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Assignment deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
