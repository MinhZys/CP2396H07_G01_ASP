using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;
using Symphony.Portal.Web.Models.ViewModels;

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

        public async Task<IActionResult> Index(int? pageNumber)
        {
            var classesQuery = _context.Classes
                .Include(c => c.ClassCategory)
                .Include(c => c.Course)
                .AsNoTracking();

            int pageSize = 10;
            var paginatedClasses = await PaginatedList<Class>.CreateAsync(classesQuery, pageNumber ?? 1, pageSize);

            // Remaining seats calculation for CURRENT PAGE only
            var remainingSeats = new Dictionary<string, int>();
            foreach (var cls in paginatedClasses)
            {
                var approvedGuests = await _context.Guests.CountAsync(g => g.ClassId == cls.Id && g.Status == GuestRegistrationStatus.Approved);
                var enrollments = await _context.Enrollments.CountAsync(e => e.ClassId == cls.Id);
                var occupied = approvedGuests + enrollments;
                remainingSeats[cls.Id] = cls.NumberOfSeats - occupied;
            }
            ViewBag.ClassRemainingSeats = remainingSeats;

            // ✅ assigned student count per class for CURRENT PAGE only
            var classIds = paginatedClasses.Select(c => c.Id).ToList();
            var assignedCounts = await _context.ClassAssignments
                .Where(a => classIds.Contains(a.ClassId))
                .GroupBy(a => a.ClassId)
                .Select(g => new { ClassId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ClassId, x => x.Count);

            ViewBag.ClassAssignedCounts = assignedCounts;

            return View(paginatedClasses);
        }


        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return NotFound();
            var @class = await _context.Classes
                .Include(c => c.ClassCategory)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (@class == null) return NotFound();

            return View(@class);
        }

        public IActionResult Create()
        {
            ViewBag.ClassCategories = new SelectList(_context.ClassCategories.Where(c => c.IsActive), "Id", "Name");
            ViewBag.Courses = new SelectList(_context.Courses.OrderBy(c => c.Title), "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClassName,ClassCategoryId,CourseId,NumberOfSeats,Status,Fee")] Class @class)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(@class.Id)) @class.Id = Guid.NewGuid().ToString();
                
                // Ensure CreatedAt is set if not bound (though model has default, validation might need it)
                @class.CreatedAt = DateTime.Now;

                _context.Add(@class);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Class created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ClassCategories = new SelectList(_context.ClassCategories.Where(c => c.IsActive), "Id", "Name", @class.ClassCategoryId);
            return View(@class);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return NotFound();
            
            ViewBag.ClassCategories = new SelectList(_context.ClassCategories.Where(c => c.IsActive), "Id", "Name", @class.ClassCategoryId);
            ViewBag.Courses = new SelectList(_context.Courses.OrderBy(c => c.Title), "Id", "Title", @class.CourseId);
            return View(@class);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ClassName,ClassCategoryId,CourseId,NumberOfSeats,Status,Fee")] Class @class)
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
                TempData["Success"] = "Class updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ClassCategories = new SelectList(_context.ClassCategories.Where(c => c.IsActive), "Id", "Name", @class.ClassCategoryId);
            return View(@class);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return NotFound();
            var @class = await _context.Classes
                .Include(c => c.ClassCategory)
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
                // Check dependencies
                if (await _context.Guests.AnyAsync(g => g.ClassId == id))
                {
                    TempData["Error"] = "Cannot delete this class because there are still guests assigned to it.";
                    return RedirectToAction(nameof(Index));
                }

                if (await _context.Assignments.AnyAsync(a => a.ClassId == id))
                {
                    TempData["Error"] = "Cannot delete this class because there are still assignments assigned to it.";
                    return RedirectToAction(nameof(Index));
                }

                if (await _context.Enrollments.AnyAsync(e => e.ClassId == id))
                {
                    TempData["Error"] = "Cannot delete this class because there are still student enrollments.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Classes.Remove(@class);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Class deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
