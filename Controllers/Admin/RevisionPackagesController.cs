using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RevisionPackagesController : Controller
    {
        private readonly AppDbContext _context;

        public RevisionPackagesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/RevisionPackages
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.RevisionPackages.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(p => p.Title.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            // Optional: sort newest first
            var list = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(list);
        }

        // GET: Admin/RevisionPackages/Create
        public IActionResult Create()
        {
            // default values
            var model = new RevisionPackage
            {
                Id = Guid.NewGuid().ToString(),
                Status = RevisionPackageStatus.Open,
                CurrentStudents = 0,
                CreatedAt = DateTime.Now
            };

            return View(model);
        }

        // POST: Admin/RevisionPackages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Fee,MaxStudents,CurrentStudents,Status")] RevisionPackage package)
        {
            // ✅ luôn set server-side
            if (string.IsNullOrWhiteSpace(package.Id))
                package.Id = Guid.NewGuid().ToString();

            package.CreatedAt = DateTime.Now;

            // normalize
            if (package.CurrentStudents < 0) package.CurrentStudents = 0;
            if (package.MaxStudents < 0) package.MaxStudents = 0;

            if (package.MaxStudents > 0 && package.CurrentStudents > package.MaxStudents)
                ModelState.AddModelError(nameof(package.CurrentStudents), "CurrentStudents cannot be greater than MaxStudents.");

            if (!ModelState.IsValid)
                return View(package);

            _context.RevisionPackages.Add(package);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Revision package created successfully.";
            return RedirectToAction(nameof(Index));
        }



        // GET: Admin/RevisionPackages/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var package = await _context.RevisionPackages.FindAsync(id);
            if (package == null) return NotFound();

            return View(package);
        }

        // POST: Admin/RevisionPackages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,
     [Bind("Id,Title,Description,Fee,MaxStudents,CurrentStudents,Status")] RevisionPackage package)
        {
            if (id != package.Id) return NotFound();

            var existing = await _context.RevisionPackages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null) return NotFound();

            // giữ CreatedAt cũ
            package.CreatedAt = existing.CreatedAt;

            if (package.CurrentStudents < 0) package.CurrentStudents = 0;
            if (package.MaxStudents < 0) package.MaxStudents = 0;

            if (package.MaxStudents > 0 && package.CurrentStudents > package.MaxStudents)
            {
                ModelState.AddModelError(nameof(package.CurrentStudents),
                    "CurrentStudents cannot be greater than MaxStudents.");
            }

            if (!ModelState.IsValid)
                return View(package);

            _context.Update(package);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Revision package updated successfully.";
            return RedirectToAction(nameof(Index));
        }


        // POST: Admin/RevisionPackages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return RedirectToAction(nameof(Index));

            var package = await _context.RevisionPackages.FindAsync(id);
            if (package != null)
            {
                if (await _context.RevisionRegistrations.AnyAsync(r => r.RevisionPackageId == id))
                {
                    TempData["Error"] = "Cannot delete this package because there are existing registrations under it.";
                    return RedirectToAction(nameof(Index));
                }

                _context.RevisionPackages.Remove(package);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Revision package deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RevisionPackageExists(string id)
        {
            return _context.RevisionPackages.Any(e => e.Id == id);
        }
    }
}
