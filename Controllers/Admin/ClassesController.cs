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
            var classes = await _context.Classes.Include(c => c.ClassCategory).ToListAsync();
            return View(classes);
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return NotFound();
            var @class = await _context.Classes
                .Include(c => c.ClassCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (@class == null) return NotFound();

            return View(@class);
        }

        public IActionResult Create()
        {
            ViewBag.ClassCategories = new SelectList(_context.ClassCategories.Where(c => c.IsActive), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClassName,ClassCategoryId,NumberOfSeats,Status")] Class @class)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(@class.Id)) @class.Id = Guid.NewGuid().ToString();
                
                // Ensure CreatedAt is set if not bound (though model has default, validation might need it)
                @class.CreatedAt = DateTime.Now;

                _context.Add(@class);
                await _context.SaveChangesAsync();
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
            return View(@class);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ClassName,ClassCategoryId,NumberOfSeats,Status")] Class @class)
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
                _context.Classes.Remove(@class);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
