using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ClassCategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public ClassCategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ClassCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClassCategories.ToListAsync());
        }

        // GET: Admin/ClassCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ClassCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,IsActive")] ClassCategory classCategory)
        {
            if (ModelState.IsValid)
            {
                classCategory.Id = Guid.NewGuid().ToString();
                _context.Add(classCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(classCategory);
        }

        // GET: Admin/ClassCategories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var classCategory = await _context.ClassCategories.FindAsync(id);
            if (classCategory == null) return NotFound();
            return View(classCategory);
        }

        // POST: Admin/ClassCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,IsActive")] ClassCategory classCategory)
        {
            if (id != classCategory.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassCategoryExists(classCategory.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(classCategory);
        }

        // GET: Admin/ClassCategories/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var classCategory = await _context.ClassCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classCategory == null) return NotFound();

            return View(classCategory);
        }

        // POST: Admin/ClassCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var classCategory = await _context.ClassCategories.FindAsync(id);
            if (classCategory != null)
            {
                _context.ClassCategories.Remove(classCategory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassCategoryExists(string id)
        {
            return _context.ClassCategories.Any(e => e.Id == id);
        }
    }
}
