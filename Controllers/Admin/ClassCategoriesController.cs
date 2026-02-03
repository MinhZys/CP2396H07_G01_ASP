using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.ViewModels;

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

        // GET: Admin/ClassCategories (with Search)
        public async Task<IActionResult> Index(string? searchString, int? pageNumber)
        {
            var categories = from c in _context.ClassCategories
                             select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(c =>
                    c.Name.Contains(searchString) ||
                    (c.Description != null && c.Description.Contains(searchString))
                );
            }

            ViewData["CurrentFilter"] = searchString;

            int pageSize = 10;
            return View(await PaginatedList<ClassCategory>.CreateAsync(
                categories.OrderByDescending(c => c.CreatedAt).AsNoTracking(), 
                pageNumber ?? 1, 
                pageSize));
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
                bool exists = await _context.ClassCategories
                    .AnyAsync(c => c.Name == classCategory.Name);

                if (exists)
                {
                    ModelState.AddModelError("Name", "Category name already exists.");
                    return View(classCategory);
                }

                classCategory.Id = Guid.NewGuid().ToString();
                classCategory.CreatedAt = DateTime.Now;

                _context.ClassCategories.Add(classCategory);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Class category created successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(classCategory);
        }

        // GET: Admin/ClassCategories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

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
                bool exists = await _context.ClassCategories
                    .AnyAsync(c => c.Name == classCategory.Name && c.Id != id);

                if (exists)
                {
                    ModelState.AddModelError("Name", "Category name already exists.");
                    return View(classCategory);
                }

                var existing = await _context.ClassCategories.FindAsync(id);
                if (existing == null) return NotFound();

                existing.Name = classCategory.Name;
                existing.Description = classCategory.Description;
                existing.IsActive = classCategory.IsActive;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Class category updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(classCategory);
        }

        // GET: Admin/ClassCategories/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

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
            if (classCategory == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Check if any classes exist in this category
            bool hasClasses = await _context.Classes
                .AnyAsync(c => c.ClassCategoryId == id);

            if (hasClasses)
            {
                TempData["Error"] =
                    "Cannot delete this category because there are still classes assigned to it. Please delete the classes first.";
                return RedirectToAction(nameof(Index));
            }

            _context.ClassCategories.Remove(classCategory);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Class category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool ClassCategoryExists(string id)
        {
            return _context.ClassCategories.Any(e => e.Id == id);
        }
    }
}
