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
    public class ClassesController : Controller
    {
        private readonly AppDbContext _context;

        public ClassesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Classes
        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes.Include(c => c.Course).Include(c => c.Instructor).ToListAsync();
            return View(classes);
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return NotFound();
            var @class = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments).ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (@class == null) return NotFound();

            return View(@class);
        }

        public IActionResult Create()
        {
            ViewBag.Courses = _context.Courses.ToList();
            ViewBag.Instructors = _context.Users.Where(u => u.Role.Name == "Instructor").ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Class @class)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(@class.Id)) @class.Id = Guid.NewGuid().ToString();

                _context.Add(@class);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Courses = _context.Courses.ToList();
            ViewBag.Instructors = _context.Users.Where(u => u.Role.Name == "Instructor").ToList();
            return View(@class);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return NotFound();
            
            ViewBag.Courses = _context.Courses.ToList();
            ViewBag.Instructors = _context.Users.Where(u => u.Role.Name == "Instructor").ToList();
            return View(@class);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Class @class)
        {
            if (id != @class.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Classes.Any(e => e.Id == @class.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Courses = _context.Courses.ToList();
            ViewBag.Instructors = _context.Users.Where(u => u.Role.Name == "Instructor").ToList();
            return View(@class);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return NotFound();
            var @class = await _context.Classes
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null) return NotFound();

            return View(@class);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Classes/PendingEnrollments
        public async Task<IActionResult> PendingEnrollments()
        {
            var pending = await _context.Enrollments
                .Include(e => e.Class).ThenInclude(c => c.Course)
                .Include(e => e.Student)
                .Where(e => !e.IsApproved)
                .OrderBy(e => e.EnrolledDate)
                .ToListAsync();
            return View(pending);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveEnrollment(string id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return NotFound();

            enrollment.IsApproved = true;
            enrollment.IsPaid = true; 
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(PendingEnrollments));
        }

        [HttpPost]
        public async Task<IActionResult> RejectEnrollment(string id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(PendingEnrollments));
        }
    }
}
